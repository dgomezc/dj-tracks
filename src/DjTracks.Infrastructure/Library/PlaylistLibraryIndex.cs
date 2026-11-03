using System.Threading.Channels;
using DjTracks.Domain;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace DjTracks.Infrastructure.Library;

public sealed class PlaylistLibraryIndex(TrackLibrary tracks, RememberLibrary remember,
    ILogger<PlaylistLibraryIndex> logger) : IDisposable
{
    private sealed class RootIndex(string library)
    {
        public string Library { get; } = library;
        public string? Root { get; set; }
        public FileSystemWatcher? Watcher { get; set; }
        public long Revision;
        public long ScannedRevision { get; set; } = -1;
        public PlaylistTrackReference[] Entries { get; set; } = [];
        public AppError? Error { get; set; }
        public int WatcherFailed;
    }

    private readonly RootIndex _tracks = new("Tracks");
    private readonly RootIndex _remember = new("Remember");
    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly Channel<byte> _changes = Channel.CreateBounded<byte>(new BoundedChannelOptions(1)
    {
        FullMode = BoundedChannelFullMode.DropWrite,
        SingleReader = true
    });

    public async Task<Result<PlaylistTrackReference[]>> ResolveAsync(PathSettings settings,
        AutomaticPlaylistFilters filters, CancellationToken ct, bool force = false)
    {
        await _gate.WaitAsync(ct);
        try
        {
            Refresh(_tracks, settings.Tracks, force, ct);
            if (_tracks.Error is not null)
                return _tracks.Error;
            if (filters.IncludeRemember)
            {
                Refresh(_remember, settings.Remember, force, ct);
                if (_remember.Error is not null)
                    return _remember.Error;
            }
            var years = filters.Years.ToHashSet(StringComparer.OrdinalIgnoreCase);
            var styles = filters.Styles.Select(AutomaticPlaylistFilters.CanonicalStyle)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            return _tracks.Entries.Where(reference =>
                    (years.Count == 0 || years.Contains(reference.Year!))
                    && (styles.Count == 0 || styles.Contains(AutomaticPlaylistFilters.CanonicalStyle(reference.Style!))))
                .Concat(filters.IncludeRemember ? _remember.Entries : [])
                .ToArray();
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task RefreshAsync(PathSettings settings, bool force, CancellationToken ct)
    {
        await _gate.WaitAsync(ct);
        try
        {
            Refresh(_tracks, settings.Tracks, force, ct);
            Refresh(_remember, settings.Remember, force, ct);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task WaitForChangesAsync(CancellationToken ct)
    {
        await _changes.Reader.ReadAsync(ct);
        // Agrupa la ráfaga de eventos de una copia o renombrado, sin trabajar en el hilo del watcher.
        await Task.Delay(TimeSpan.FromMilliseconds(250), ct);
        while (_changes.Reader.TryRead(out _)) { }
    }

    public void ConfigurationChanged()
    {
        Invalidate(_tracks);
        Invalidate(_remember);
    }

    private void Refresh(RootIndex index, string root, bool force, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        if (!string.Equals(root, index.Root, StringComparison.Ordinal))
        {
            index.Watcher?.Dispose();
            index.Watcher = null;
            index.Root = root;
            index.Entries = [];
            index.ScannedRevision = -1;
            Interlocked.Increment(ref index.Revision);
        }
        if (string.IsNullOrWhiteSpace(root))
        {
            index.Error = new AppError(ErrorKind.Conflict, "Configura la biblioteca.",
                $"Configura la ruta de {index.Library} antes de usarla en una playlist automática.");
            return;
        }
        try
        {
            // Una raíz desconectada no equivale a cero canciones, aunque el watcher no avise.
            using (var probe = Directory.EnumerateFileSystemEntries(root).GetEnumerator())
                probe.MoveNext();
            if (Interlocked.Exchange(ref index.WatcherFailed, 0) != 0)
            {
                index.Watcher?.Dispose();
                index.Watcher = null;
            }
            if (index.Watcher is null)
                StartWatcher(index, root);
            var revision = Interlocked.Read(ref index.Revision);
            if (!force && index.Error is null && index.ScannedRevision == revision)
                return;

            var result = index.Library == "Tracks"
                ? Result.Ok(tracks.IndexReferences(root, ct))
                : remember.IndexReferences(root, ct);
            if (result.IsFailed)
            {
                logger.LogWarning("No se pudo indexar {Library}: {Error}", index.Library, result.Errors[0].Message);
                index.Error = new AppError(ErrorKind.Unavailable, "Biblioteca no disponible.",
                    $"No se pudo indexar {index.Library}. {result.Errors[0].Message} No se ha actualizado la playlist.");
                return;
            }
            index.Entries = result.Value;
            index.Error = null;
            index.ScannedRevision = revision;
            logger.LogInformation("Índice de {Library} actualizado: {Count} canciones", index.Library, index.Entries.Length);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException
            or ArgumentException or NotSupportedException)
        {
            logger.LogError(ex, "No se pudo actualizar el índice de {Library}", index.Library);
            index.Error = new AppError(ErrorKind.Unavailable, "Biblioteca no disponible.",
                $"No se pudo leer {index.Library}. Comprueba su ruta, permisos y almacenamiento y vuelve a cargar. No se ha actualizado la playlist.");
            index.Watcher?.Dispose();
            index.Watcher = null;
        }
    }

    private void StartWatcher(RootIndex index, string root)
    {
        FileSystemWatcher? watcher = null;
        try
        {
            watcher = new FileSystemWatcher(root)
            {
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite
                    | NotifyFilters.Size | NotifyFilters.Attributes
            };
            watcher.Created += (_, _) => Invalidate(index);
            watcher.Deleted += (_, _) => Invalidate(index);
            watcher.Renamed += (_, _) => Invalidate(index);
            watcher.Changed += (_, _) => Invalidate(index);
            watcher.Error += (_, args) =>
            {
                logger.LogWarning(args.GetException(),
                    "Watcher de {Library} interrumpido; se reindexará y se recreará la vigilancia", index.Library);
                Interlocked.Exchange(ref index.WatcherFailed, 1);
                Invalidate(index);
            };
            watcher.EnableRaisingEvents = true;
            index.Watcher = watcher;
            // Vigilar antes de escanear impide perder cambios entre el escaneo inicial y la suscripción.
            Interlocked.Increment(ref index.Revision);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException
            or ArgumentException or NotSupportedException)
        {
            watcher?.Dispose();
            logger.LogWarning(ex, "Watcher de {Library} no disponible; se usará el reescaneo horario", index.Library);
        }
    }

    private void Invalidate(RootIndex index)
    {
        Interlocked.Increment(ref index.Revision);
        _changes.Writer.TryWrite(0);
    }

    public void Dispose()
    {
        _tracks.Watcher?.Dispose();
        _remember.Watcher?.Dispose();
    }
}
