using System.Text;
using DjTracks.Domain;
using DjTracks.Infrastructure.Library;
using DjTracks.Infrastructure.Settings;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace DjTracks.Infrastructure.Playlists;

public sealed class PlaylistM3uExporter(JsonPlaylistStore playlists, JsonSettingsStore settingsStore,
    TrackLibrary tracks, RememberLibrary remember, ILogger<PlaylistM3uExporter> logger)
{
    private static readonly char[] InvalidFileNameCharacters = ['<', '>', ':', '"', '/', '\\', '|', '?', '*'];
    private static readonly HashSet<string> ReservedFileNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "CON", "PRN", "AUX", "NUL",
        "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
        "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
    };

    public async Task<Result<PlaylistM3uExport>> ExportAsync(Guid id, CancellationToken ct)
    {
        try
        {
            var playlistResult = await playlists.GetAsync(id, ct);
            if (playlistResult.IsFailed)
                return playlistResult.ToResult();

            var settingsResult = await settingsStore.LoadAsync(ct);
            if (settingsResult.IsFailed)
                return settingsResult.ToResult();
            var settings = settingsResult.Value;
            if (string.IsNullOrWhiteSpace(settings.M3uRoot))
                return new AppError(ErrorKind.Conflict, "Configura la ruta M3U.",
                    "Abre Configuración y guarda una carpeta de exportación existente y escribible.");

            var root = Path.GetFullPath(settings.M3uRoot);
            using (var entries = Directory.EnumerateFileSystemEntries(root).GetEnumerator())
                entries.MoveNext();

            List<string> lines = ["#EXTM3U"];
            foreach (var entry in playlistResult.Value.Entries)
            {
                ct.ThrowIfCancellationRequested();
                var reference = entry.Track;
                var file = reference.Library == "Tracks"
                    ? tracks.ResolveTrackFile(settings.Tracks, reference.Year!, reference.Style!, reference.FileName, ct)
                    : remember.ResolveAudioFile(settings.Remember, reference.Category!, reference.FileName, ct);
                if (file.IsFailed)
                    return file.ToResult();

                var relativePath = Path.GetRelativePath(root, file.Value.FullName);
                if (Path.IsPathRooted(relativePath))
                    return new AppError(ErrorKind.Conflict, "No se puede crear una ruta relativa.",
                        "La raíz M3U y la biblioteca están en unidades distintas. Configúralas en la misma unidad para exportar.");
                relativePath = relativePath.Replace(Path.DirectorySeparatorChar, '/')
                    .Replace(Path.AltDirectorySeparatorChar, '/');
                if (relativePath.StartsWith('#'))
                    relativePath = "./" + relativePath;

                var details = TrackLibrary.ReadTrack(file.Value, ct);
                var title = string.IsNullOrWhiteSpace(details.Title)
                    ? Path.GetFileNameWithoutExtension(reference.FileName)
                    : details.Title;
                var artist = string.Join(", ", details.Artists.Where(value => !string.IsNullOrWhiteSpace(value)));
                var label = CleanMetadata(string.IsNullOrWhiteSpace(artist) ? title : $"{artist} - {title}");
                var duration = details.DurationSeconds is { } seconds && double.IsFinite(seconds) && seconds >= 0
                    ? Math.Max(0, (int)Math.Round(seconds))
                    : -1;
                lines.Add($"#EXTINF:{duration},{label}");
                lines.Add(relativePath);
            }

            var allPlaylists = await playlists.ListAsync(ct);
            if (allPlaylists.IsFailed)
                return allPlaylists.ToResult();
            var stem = SafeFileStem(playlistResult.Value.Name);
            var hasSanitizedCollision = allPlaylists.Value.Any(other => other.Id != id
                && SafeFileStem(other.Name).Equals(stem, StringComparison.OrdinalIgnoreCase));
            var fileName = hasSanitizedCollision ? $"{stem}-{id:N}.m3u" : $"{stem}.m3u";
            var destination = Path.Combine(root, fileName);
            if (File.Exists(destination) && (File.GetAttributes(destination) & FileAttributes.ReparsePoint) != 0)
                return new AppError(ErrorKind.Conflict, "Destino M3U no válido.",
                    "El archivo de destino es un enlace y no se sobrescribió.");

            var temporary = Path.Combine(root, $".djtracks-{Guid.NewGuid():N}.tmp");
            try
            {
                var content = Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, lines) + Environment.NewLine);
                await using (var stream = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write,
                    FileShare.None, 4096, FileOptions.Asynchronous))
                {
                    await stream.WriteAsync(content, ct);
                    await stream.FlushAsync(ct);
                    stream.Flush(flushToDisk: true);
                }
                ct.ThrowIfCancellationRequested();
                File.Move(temporary, destination, overwrite: true);
            }
            finally
            {
                if (File.Exists(temporary))
                    File.Delete(temporary);
            }

            return new PlaylistM3uExport(fileName, playlistResult.Value.Entries.Length);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException
            or ArgumentException or NotSupportedException)
        {
            logger.LogError(ex, "No se pudo exportar la playlist {PlaylistId} a M3U", id);
            return new AppError(ErrorKind.Unavailable, "No se pudo exportar el M3U.",
                "Comprueba la ruta M3U, sus permisos de escritura y que el almacenamiento esté disponible.");
        }
    }

    private static string SafeFileStem(string name)
    {
        var stem = new string(name.Select(character =>
            char.IsControl(character) || InvalidFileNameCharacters.Contains(character) ? '_' : character).ToArray())
            .Trim().TrimEnd('.');
        if (stem.Length == 0)
            stem = "playlist";
        if (ReservedFileNames.Contains(stem))
            stem = $"_{stem}";
        return stem;
    }

    private static string CleanMetadata(string value) =>
        string.Concat(value.Select(character => char.IsControl(character) ? ' ' : character)).Trim();
}
