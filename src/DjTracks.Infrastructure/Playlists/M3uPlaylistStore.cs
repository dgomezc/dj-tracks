using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using DjTracks.Domain;
using DjTracks.Infrastructure.Library;
using DjTracks.Infrastructure.Settings;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace DjTracks.Infrastructure.Playlists;

// Cada playlist es un `<nombre>.m3u` (UTF-8) en la carpeta de playlists, con rutas relativas a ella.
// `#PLAYLIST` guarda el nombre visible y `#DJTRACKS-ID` el identificador estable.
public sealed class M3uPlaylistStore(JsonSettingsStore settingsStore, TrackLibrary tracks,
    RememberLibrary remember, ILogger<M3uPlaylistStore> logger)
{
    private const string NamePrefix = "#PLAYLIST:";
    private const string IdPrefix = "#DJTRACKS-ID:";
    private const string InfoPrefix = "#EXTINF:";
    private static readonly char[] InvalidFileNameCharacters = ['<', '>', ':', '"', '/', '\\', '|', '?', '*'];
    private static readonly HashSet<string> ReservedFileNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "CON", "PRN", "AUX", "NUL",
        "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
        "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
    };
    private static readonly JsonSerializerOptions LegacyJsonOptions = new(JsonSerializerDefaults.Web);

    private readonly SemaphoreSlim _gate = new(1, 1);

    private sealed record StoredPlaylist(ManualPlaylist Playlist, string Path,
        Dictionary<PlaylistTrackReference, string> Info);

    public Task<Result<ManualPlaylist[]>> ListAsync(CancellationToken ct) =>
        ExecuteAsync<ManualPlaylist[]>(async settings => (await ReadAllAsync(settings, ct))
            .Select(item => item.Playlist)
            .OrderBy(item => item.Name, StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id).ToArray(), ct);

    public Task<Result<ManualPlaylist>> GetAsync(Guid id, CancellationToken ct) =>
        ExecuteAsync<ManualPlaylist>(async settings =>
        {
            var result = await FindAsync(settings, id, ct);
            return result.IsFailed ? result.ToResult() : result.Value.Playlist;
        }, ct);

    public Task<Result<ManualPlaylist>> CreateAsync(string name, CancellationToken ct) =>
        ExecuteAsync<ManualPlaylist>(async settings =>
        {
            var all = await ReadAllAsync(settings, ct);
            var path = TargetPath(settings.Playlists, name);
            if (IsTaken(all, name, path, null))
                return DuplicateName();
            var playlist = new ManualPlaylist(Guid.NewGuid(), name, []);
            await WriteAsync(settings, playlist, [], path, overwrite: false, ct);
            return playlist;
        }, ct);

    public Task<Result<ManualPlaylist>> RenameAsync(Guid id, string name, CancellationToken ct) =>
        ExecuteAsync<ManualPlaylist>(async settings =>
        {
            var all = await ReadAllAsync(settings, ct);
            var stored = all.FirstOrDefault(item => item.Playlist.Id == id);
            if (stored is null)
                return NotFound();
            var path = TargetPath(settings.Playlists, name);
            if (IsTaken(all, name, path, stored))
                return DuplicateName();
            var playlist = stored.Playlist with { Name = name };
            if (path.Equals(stored.Path, StringComparison.OrdinalIgnoreCase))
            {
                await WriteAsync(settings, playlist, stored.Info, stored.Path, overwrite: true, ct);
                if (!Path.GetFileName(path).Equals(Path.GetFileName(stored.Path), StringComparison.Ordinal))
                    File.Move(stored.Path, path);
                return playlist;
            }
            await WriteAsync(settings, playlist, stored.Info, path, overwrite: false, ct);
            try
            {
                File.Delete(stored.Path);
            }
            catch
            {
                File.Delete(path);
                throw;
            }
            return playlist;
        }, ct);

    public Task<Result<Guid>> DeleteAsync(Guid id, CancellationToken ct) =>
        ExecuteAsync<Guid>(async settings =>
        {
            var result = await FindAsync(settings, id, ct);
            if (result.IsFailed)
                return result.ToResult();
            ct.ThrowIfCancellationRequested();
            File.Delete(result.Value.Path);
            return id;
        }, ct);

    public Task<Result<ManualPlaylist>> AddAsync(Guid id, PlaylistTrackReference track, CancellationToken ct) =>
        ExecuteAsync<ManualPlaylist>(async settings =>
        {
            var result = await FindAsync(settings, id, ct);
            if (result.IsFailed)
                return result.ToResult();
            var stored = result.Value;
            if (stored.Playlist.Entries.Any(entry => entry.Track == track))
                return stored.Playlist;
            var playlist = stored.Playlist with { Entries = [.. stored.Playlist.Entries, NewEntry(track)] };
            await WriteAsync(settings, playlist, stored.Info, stored.Path, overwrite: true, ct);
            return playlist;
        }, ct);

    public Task<Result<ManualPlaylist>> RemoveAsync(Guid id, Guid entryId, CancellationToken ct) =>
        ExecuteAsync<ManualPlaylist>(async settings =>
        {
            var result = await FindAsync(settings, id, ct);
            if (result.IsFailed)
                return result.ToResult();
            var stored = result.Value;
            if (!stored.Playlist.Entries.Any(entry => entry.Id == entryId))
                return new AppError(ErrorKind.NotFound, "Canción no encontrada.",
                    "La entrada ya no está en la playlist. Vuelve a cargarla.");
            var playlist = stored.Playlist with
            {
                Entries = stored.Playlist.Entries.Where(entry => entry.Id != entryId).ToArray()
            };
            await WriteAsync(settings, playlist, stored.Info, stored.Path, overwrite: true, ct);
            return playlist;
        }, ct);

    public static bool IsValidName(string? name) =>
        !string.IsNullOrWhiteSpace(name) && name.Trim().Length <= 120 && !name.Any(char.IsControl);

    private async Task<Result<T>> ExecuteAsync<T>(Func<PathSettings, Task<Result<T>>> operation, CancellationToken ct)
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
            // No crear una carpeta ausente: podría tratarse de almacenamiento desconectado.
            using (var entries = Directory.EnumerateFileSystemEntries(settings.Value.Playlists).GetEnumerator())
                entries.MoveNext();
            await MigrateLegacyAsync(settings.Value, ct);
            var result = await operation(settings.Value);
            if (result.IsFailed)
                logger.LogWarning("Operación de playlists rechazada: {Error}", result.Errors[0].Message);
            return result;
        }
        catch (InvalidDataException ex)
        {
            logger.LogError(ex, "Playlist M3U inválida");
            return new AppError(ErrorKind.Unexpected, "Playlist M3U inválida.",
                $"{ex.Message} Corrige o restaura el archivo en el servidor; no se ha sobrescrito.");
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Playlist JSON antigua inválida");
            return new AppError(ErrorKind.Unexpected, "Playlist JSON inválida.",
                "Una playlist JSON antigua no se pudo convertir a M3U. Corrige o elimina el archivo en el servidor; no se ha modificado.");
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException
            or ArgumentException or NotSupportedException)
        {
            logger.LogError(ex, "No se pudo acceder al almacenamiento de playlists");
            return new AppError(ErrorKind.Unavailable, "Playlists no disponibles.",
                "Comprueba la ruta de playlists, sus permisos de lectura/escritura y el almacenamiento. No se ha confirmado ningún cambio.");
        }
        finally
        {
            _gate.Release();
        }
    }

    private async Task<Result<StoredPlaylist>> FindAsync(PathSettings settings, Guid id, CancellationToken ct)
    {
        if (id == Guid.Empty)
            return new AppError(ErrorKind.Validation, "Identificador inválido.", "Selecciona una playlist válida.");
        var stored = (await ReadAllAsync(settings, ct)).FirstOrDefault(item => item.Playlist.Id == id);
        return stored is null ? NotFound() : stored;
    }

    private async Task<StoredPlaylist[]> ReadAllAsync(PathSettings settings, CancellationToken ct)
    {
        List<StoredPlaylist> playlists = [];
        foreach (var file in new DirectoryInfo(settings.Playlists).EnumerateFiles("*.m3u"))
        {
            if (!file.Extension.Equals(".m3u", StringComparison.OrdinalIgnoreCase))
                continue;
            if ((file.Attributes & FileAttributes.ReparsePoint) != 0)
                throw new IOException("No se permiten enlaces en los archivos de playlists.");
            var stored = await ReadAsync(settings, file, ct);
            if (playlists.Any(item => item.Playlist.Id == stored.Playlist.Id))
                throw new InvalidDataException($"«{file.Name}» repite el identificador de otra playlist.");
            playlists.Add(stored);
        }
        return playlists.ToArray();
    }

    private static async Task<StoredPlaylist> ReadAsync(PathSettings settings, FileInfo file, CancellationToken ct)
    {
        var lines = await File.ReadAllLinesAsync(file.FullName, Encoding.UTF8, ct);
        Guid? id = null;
        string? name = null;
        string? info = null;
        List<PlaylistEntry> entries = [];
        Dictionary<PlaylistTrackReference, string> infos = [];
        foreach (var raw in lines)
        {
            var line = raw.Trim();
            if (line.Length == 0)
                continue;
            if (line.StartsWith(InfoPrefix, StringComparison.OrdinalIgnoreCase))
                info = line;
            else if (line.StartsWith(NamePrefix, StringComparison.OrdinalIgnoreCase))
                name = line[NamePrefix.Length..].Trim();
            else if (line.StartsWith(IdPrefix, StringComparison.OrdinalIgnoreCase))
                id = Guid.TryParseExact(line[IdPrefix.Length..].Trim(), "N", out var parsed) && parsed != Guid.Empty
                    ? parsed
                    : throw new InvalidDataException($"«{file.Name}» tiene un identificador inválido.");
            else if (!line.StartsWith('#'))
            {
                var reference = ToReference(settings, line)
                    ?? throw new InvalidDataException(
                        $"«{file.Name}» contiene «{line}», que no es una canción de Tracks (año/estilo/archivo.mp3) ni de Remember (Cortas|Largas/archivo.mp3).");
                if (entries.All(entry => entry.Track != reference))
                {
                    entries.Add(NewEntry(reference));
                    if (info is not null)
                        infos[reference] = info;
                }
                info = null;
            }
        }
        name ??= Path.GetFileNameWithoutExtension(file.Name);
        if (!IsValidName(name))
            throw new InvalidDataException($"«{file.Name}» tiene un nombre de playlist inválido.");
        var playlist = new ManualPlaylist(id ?? StableGuid("file:" + file.Name.ToLowerInvariant()),
            name.Trim(), entries.ToArray());
        return new StoredPlaylist(playlist, file.FullName, infos);
    }

    private static PlaylistTrackReference? ToReference(PathSettings settings, string line)
    {
        var value = line.StartsWith("file:", StringComparison.OrdinalIgnoreCase)
            && Uri.TryCreate(line, UriKind.Absolute, out var uri) && uri.IsFile
            ? uri.LocalPath
            : line.Replace('/', Path.DirectorySeparatorChar);
        var full = Path.GetFullPath(Path.IsPathRooted(value) ? value : Path.Combine(settings.Playlists, value));

        if (Segments(settings.Tracks, full) is [var year, var style, var trackFile])
        {
            var reference = new PlaylistTrackReference("Tracks", trackFile, year, style);
            if (reference.IsValid())
                return reference;
        }
        if (Segments(settings.Remember, full) is [var category, var rememberFile])
        {
            category = category.Equals("Cortas", StringComparison.OrdinalIgnoreCase) ? "Cortas"
                : category.Equals("Largas", StringComparison.OrdinalIgnoreCase) ? "Largas" : category;
            var reference = new PlaylistTrackReference("Remember", rememberFile, Category: category);
            if (reference.IsValid())
                return reference;
        }
        return null;
    }

    private static string[]? Segments(string root, string full)
    {
        var relative = Path.GetRelativePath(Path.GetFullPath(root), full);
        return relative == "." || Path.IsPathRooted(relative) || relative == ".."
            || relative.StartsWith(".." + Path.DirectorySeparatorChar, StringComparison.Ordinal)
            ? null
            : relative.Split(Path.DirectorySeparatorChar);
    }

    private async Task WriteAsync(PathSettings settings, ManualPlaylist playlist,
        Dictionary<PlaylistTrackReference, string> infos, string path, bool overwrite, CancellationToken ct)
    {
        var root = Path.GetFullPath(settings.Playlists);
        StringBuilder content = new();
        content.AppendLine("#EXTM3U").AppendLine(NamePrefix + playlist.Name).AppendLine($"{IdPrefix}{playlist.Id:N}");
        foreach (var entry in playlist.Entries)
        {
            ct.ThrowIfCancellationRequested();
            var reference = entry.Track;
            var full = reference.Library == "Tracks"
                ? Path.Combine(settings.Tracks, reference.Year!, reference.Style!, reference.FileName)
                : Path.Combine(settings.Remember, reference.Category!, reference.FileName);
            // En unidades distintas no existe ruta relativa: se guarda la absoluta.
            var location = Path.GetRelativePath(root, Path.GetFullPath(full));
            if (!Path.IsPathRooted(location))
                location = location.Replace(Path.DirectorySeparatorChar, '/');
            if (location.StartsWith('#'))
                location = "./" + location;
            content.AppendLine(infos.TryGetValue(reference, out var info) ? info : ReadInfo(settings, reference, ct));
            content.AppendLine(location);
        }

        var temporary = Path.Combine(root, $".djtracks-{Guid.NewGuid():N}.tmp");
        try
        {
            await using (var stream = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write,
                FileShare.None, 4096, FileOptions.Asynchronous))
            {
                await stream.WriteAsync(new UTF8Encoding(false).GetBytes(content.ToString()), ct);
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

    private string ReadInfo(PathSettings settings, PlaylistTrackReference reference, CancellationToken ct)
    {
        var title = Path.GetFileNameWithoutExtension(reference.FileName);
        var file = reference.Library == "Tracks"
            ? tracks.ResolveTrackFile(settings.Tracks, reference.Year!, reference.Style!, reference.FileName, ct)
            : remember.ResolveAudioFile(settings.Remember, reference.Category!, reference.FileName, ct);
        if (file.IsFailed)
            return $"{InfoPrefix}-1,{title}";
        var details = TrackLibrary.ReadTrack(file.Value, ct);
        if (!string.IsNullOrWhiteSpace(details.Title))
            title = details.Title;
        var artist = string.Join(", ", details.Artists.Where(value => !string.IsNullOrWhiteSpace(value)));
        var label = string.Concat((string.IsNullOrWhiteSpace(artist) ? title : $"{artist} - {title}")
            .Select(character => char.IsControl(character) ? ' ' : character)).Trim();
        var duration = details.DurationSeconds is { } seconds && double.IsFinite(seconds) && seconds >= 0
            ? (int)Math.Round(seconds)
            : -1;
        return $"{InfoPrefix}{duration},{label}";
    }

    // Convierte una sola vez las playlists JSON de versiones anteriores y borra el JSON tras escribir el M3U.
    private async Task MigrateLegacyAsync(PathSettings settings, CancellationToken ct)
    {
        var legacy = new DirectoryInfo(settings.Playlists).EnumerateFiles("*.json")
            .Where(file => Guid.TryParseExact(Path.GetFileNameWithoutExtension(file.Name), "N", out _))
            .ToArray();
        if (legacy.Length == 0)
            return;
        var existing = await ReadAllAsync(settings, ct);
        foreach (var file in legacy)
        {
            ManualPlaylist? playlist;
            await using (var stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
                playlist = await JsonSerializer.DeserializeAsync<ManualPlaylist>(stream, LegacyJsonOptions, ct);
            if (playlist is null || !IsValidName(playlist.Name) || playlist.Entries is null
                || playlist.Entries.Any(entry => entry?.Track is null || !entry.Track.IsValid()))
                throw new JsonException("La playlist JSON antigua no respeta su contrato.");
            if (existing.All(item => item.Playlist.Id != playlist.Id))
            {
                var converted = playlist with
                {
                    Entries = playlist.Entries.Select(entry => entry.Track).Distinct().Select(NewEntry).ToArray()
                };
                var path = TargetPath(settings.Playlists, playlist.Name);
                if (File.Exists(path))
                    path = Path.Combine(settings.Playlists,
                        $"{Path.GetFileNameWithoutExtension(path)}-{playlist.Id:N}.m3u");
                await WriteAsync(settings, converted, [], path, overwrite: false, ct);
                logger.LogInformation("Playlist JSON {PlaylistId} convertida a M3U", playlist.Id);
            }
            File.Delete(file.FullName);
        }
    }

    private static bool IsTaken(StoredPlaylist[] all, string name, string path, StoredPlaylist? except) =>
        all.Any(item => item != except && (item.Playlist.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
            || item.Path.Equals(path, StringComparison.OrdinalIgnoreCase)));

    private static string TargetPath(string root, string name)
    {
        var stem = new string(name.Select(character =>
            char.IsControl(character) || InvalidFileNameCharacters.Contains(character) ? '_' : character).ToArray())
            .Trim().TrimEnd('.');
        if (stem.Length == 0)
            stem = "playlist";
        if (ReservedFileNames.Contains(stem))
            stem = $"_{stem}";
        return Path.Combine(Path.GetFullPath(root), $"{stem}.m3u");
    }

    private static PlaylistEntry NewEntry(PlaylistTrackReference track) =>
        new(StableGuid($"{track.Library}|{track.Year}|{track.Style}|{track.Category}|{track.FileName}"), track);

    private static Guid StableGuid(string value) => new(MD5.HashData(Encoding.UTF8.GetBytes(value)));

    private static AppError NotFound() => new(ErrorKind.NotFound, "Playlist no encontrada.",
        "La playlist ya no existe. Vuelve a cargar el listado.");

    private static AppError DuplicateName() => new(ErrorKind.Conflict, "Nombre repetido.",
        "Ya existe una playlist con ese nombre (o con un nombre de archivo equivalente). Elige otro.");
}
