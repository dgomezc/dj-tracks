using System.Text.Json;
using DjTracks.Domain;
using DjTracks.Infrastructure.Settings;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace DjTracks.Infrastructure.Playlists;

public sealed class JsonPlaylistStore(JsonSettingsStore settingsStore, ILogger<JsonPlaylistStore> logger)
{
    private readonly SemaphoreSlim _gate = new(1, 1);
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web) { WriteIndented = true };

    public Task<Result<ManualPlaylist[]>> ListAsync(CancellationToken ct) =>
        ExecuteAsync<ManualPlaylist[]>(async root => (await ReadAllAsync(root, ct))
            .OrderBy(item => item.Name, StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id).ToArray(), ct);

    public Task<Result<ManualPlaylist>> GetAsync(Guid id, CancellationToken ct) =>
        ExecuteAsync(root => ReadAsync(root, id, ct), ct);

    public Task<Result<ManualPlaylist>> CreateAsync(string name, CancellationToken ct) =>
        ExecuteAsync<ManualPlaylist>(async root =>
        {
            if (await HasNameAsync(root, name, null, ct))
                return DuplicateName();
            var playlist = new ManualPlaylist(Guid.NewGuid(), name, []);
            await WriteAsync(root, playlist, overwrite: false, ct);
            return playlist;
        }, ct);

    public Task<Result<ManualPlaylist>> RenameAsync(Guid id, string name, CancellationToken ct) =>
        ExecuteAsync<ManualPlaylist>(async root =>
        {
            var result = await ReadAsync(root, id, ct);
            if (result.IsFailed)
                return result;
            if (await HasNameAsync(root, name, id, ct))
                return DuplicateName();
            var playlist = result.Value with { Name = name };
            await WriteAsync(root, playlist, overwrite: true, ct);
            return playlist;
        }, ct);

    public Task<Result<Guid>> DeleteAsync(Guid id, CancellationToken ct) =>
        ExecuteAsync<Guid>(async root =>
        {
            var result = await ReadAsync(root, id, ct);
            if (result.IsFailed)
                return result.ToResult();
            ct.ThrowIfCancellationRequested();
            File.Delete(FilePath(root, id));
            return id;
        }, ct);

    public Task<Result<ManualPlaylist>> AddAsync(Guid id, PlaylistTrackReference track, CancellationToken ct) =>
        ExecuteAsync<ManualPlaylist>(async root =>
        {
            var result = await ReadAsync(root, id, ct);
            if (result.IsFailed || result.Value.Entries.Any(entry => entry.Track == track))
                return result;
            var playlist = result.Value with { Entries = [.. result.Value.Entries, new(Guid.NewGuid(), track)] };
            await WriteAsync(root, playlist, overwrite: true, ct);
            return playlist;
        }, ct);

    public Task<Result<ManualPlaylist>> RemoveAsync(Guid id, Guid entryId, CancellationToken ct) =>
        ExecuteAsync<ManualPlaylist>(async root =>
        {
            var result = await ReadAsync(root, id, ct);
            if (result.IsFailed)
                return result;
            if (!result.Value.Entries.Any(entry => entry.Id == entryId))
                return new AppError(ErrorKind.NotFound, "Canción no encontrada.",
                    "La entrada ya no está en la playlist. Vuelve a cargarla.");
            var playlist = result.Value with { Entries = result.Value.Entries.Where(entry => entry.Id != entryId).ToArray() };
            await WriteAsync(root, playlist, overwrite: true, ct);
            return playlist;
        }, ct);

    private async Task<Result<T>> ExecuteAsync<T>(Func<string, Task<Result<T>>> operation, CancellationToken ct)
    {
        await _gate.WaitAsync(ct);
        try
        {
            var settings = await settingsStore.LoadAsync(ct);
            if (settings.IsFailed)
                return settings.ToResult();
            if (string.IsNullOrWhiteSpace(settings.Value.Playlists))
                return new AppError(ErrorKind.Conflict, "Configura las playlists.",
                    "Abre Configuración y guarda las rutas antes de usar playlists.");
            var root = settings.Value.Playlists;
            // No crear una carpeta ausente: podría tratarse de almacenamiento desconectado.
            using (var entries = Directory.EnumerateFileSystemEntries(root).GetEnumerator())
                entries.MoveNext();
            var result = await operation(root);
            if (result.IsFailed)
                logger.LogWarning("Operación de playlists rechazada: {Error}", result.Errors[0].Message);
            return result;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            logger.LogError(ex, "No se pudo acceder al almacenamiento de playlists");
            return ex is JsonException
                ? new AppError(ErrorKind.Unexpected, "Playlist JSON inválida.",
                    "Una playlist contiene JSON o datos inválidos. Corrige o restaura el archivo en el servidor; no se ha sobrescrito.")
                : new AppError(ErrorKind.Unavailable, "Playlists no disponibles.",
                    "Comprueba la ruta de playlists, sus permisos de lectura/escritura y el almacenamiento. No se ha confirmado ningún cambio.");
        }
        finally
        {
            _gate.Release();
        }
    }

    private async Task<ManualPlaylist[]> ReadAllAsync(string root, CancellationToken ct)
    {
        List<ManualPlaylist> playlists = [];
        foreach (var file in new DirectoryInfo(root).EnumerateFiles("*.json"))
        {
            if (!Guid.TryParseExact(Path.GetFileNameWithoutExtension(file.Name), "N", out var id))
                continue;
            var result = await ReadAsync(root, id, ct);
            if (result.IsFailed)
                throw new IOException("La playlist desapareció durante la lectura.");
            playlists.Add(result.Value);
        }
        return playlists.ToArray();
    }

    private async Task<Result<ManualPlaylist>> ReadAsync(string root, Guid id, CancellationToken ct)
    {
        if (id == Guid.Empty)
            return new AppError(ErrorKind.Validation, "Identificador inválido.", "Selecciona una playlist válida.");
        var path = FilePath(root, id);
        FileStream stream;
        try
        {
            if ((File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0)
                throw new IOException("No se permiten enlaces en los archivos de playlists.");
            stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Delete);
        }
        catch (FileNotFoundException)
        {
            return new AppError(ErrorKind.NotFound, "Playlist no encontrada.",
                "La playlist ya no existe. Vuelve a cargar el listado.");
        }
        await using (stream)
        {
            var playlist = await JsonSerializer.DeserializeAsync<ManualPlaylist>(stream, JsonOptions, ct);
            if (playlist is null || playlist.Id != id || !IsValidName(playlist.Name)
                || playlist.Entries is null || playlist.Entries.Any(entry => entry is null
                    || entry.Id == Guid.Empty || entry.Track is null || !entry.Track.IsValid())
                || playlist.Entries.Select(entry => entry.Id).Distinct().Count() != playlist.Entries.Length
                || playlist.Entries.Select(entry => entry.Track).Distinct().Count() != playlist.Entries.Length)
                throw new JsonException("La playlist no respeta su contrato.");
            return playlist;
        }
    }

    private async Task<bool> HasNameAsync(string root, string name, Guid? except, CancellationToken ct) =>
        (await ReadAllAsync(root, ct)).Any(item => item.Id != except
            && item.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

    private static async Task WriteAsync(string root, ManualPlaylist playlist, bool overwrite, CancellationToken ct)
    {
        var path = FilePath(root, playlist.Id);
        var temporary = Path.Combine(root, $".djtracks-{Guid.NewGuid():N}.tmp");
        try
        {
            await using (var stream = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write,
                FileShare.None, 4096, FileOptions.Asynchronous))
            {
                await JsonSerializer.SerializeAsync(stream, playlist, JsonOptions, ct);
                await stream.FlushAsync(ct);
                stream.Flush(flushToDisk: true);
            }
            ct.ThrowIfCancellationRequested();
            File.Move(temporary, path, overwrite);
        }
        finally
        {
            if (File.Exists(temporary))
                File.Delete(temporary);
        }
    }

    public static bool IsValidName(string? name) =>
        !string.IsNullOrWhiteSpace(name) && name.Trim().Length <= 120 && !name.Any(char.IsControl);

    private static string FilePath(string root, Guid id) => Path.Combine(root, $"{id:N}.json");
    private static AppError DuplicateName() => new(ErrorKind.Conflict, "Nombre repetido.",
        "Ya existe una playlist con ese nombre. Elige otro.");
}
