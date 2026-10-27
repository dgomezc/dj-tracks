using DjTracks.Domain;
using FluentResults;

namespace DjTracks.Infrastructure.Library;

public sealed class TrackLibrary
{
    // year y style llegan validados como nombres de carpeta (GetTracksRequestValidator).
    public Result<TrackListing> Browse(string root, string? year, string? style, CancellationToken ct)
    {
        var directory = new DirectoryInfo(root);
        foreach (var name in new[] { year, style }.OfType<string>())
        {
            var child = FindChild(directory, name);
            if (child is null)
                return new AppError(ErrorKind.NotFound, "Carpeta no encontrada.",
                    "La carpeta seleccionada ya no existe. Vuelve al nivel anterior.");
            directory = child;
        }

        ct.ThrowIfCancellationRequested();
        if (style is null)
        {
            var folders = directory.EnumerateDirectories()
                .Where(IsRegularEntry)
                .Select(folder => folder.Name)
                .OrderBy(name => name, StringComparer.OrdinalIgnoreCase).ToArray();
            if (year is null)
                Array.Reverse(folders);
            return new TrackListing(year, style, folders, []);
        }

        List<TrackFile> tracks = [];
        foreach (var file in directory.EnumerateFiles()
            .Where(file => IsRegularEntry(file)
                && file.Extension.Equals(".mp3", StringComparison.OrdinalIgnoreCase))
            .OrderBy(file => file.Name, StringComparer.OrdinalIgnoreCase))
        {
            ct.ThrowIfCancellationRequested();
            try
            {
                using var audio = TagLib.File.Create(file.FullName);
                tracks.Add(new(file.Name, audio.Tag.Title, audio.Tag.Performers,
                    audio.Tag.Album, audio.Properties.Duration.TotalSeconds, null));
            }
            catch (Exception ex) when (ex is TagLib.CorruptFileException or TagLib.UnsupportedFormatException
                or IOException or UnauthorizedAccessException)
            {
                tracks.Add(new(file.Name, null, [], null, null,
                    "No se pudieron leer los tags o el audio. Comprueba el fichero y sus permisos."));
            }
        }
        return new TrackListing(year, style, [], tracks.ToArray());
    }

    private static DirectoryInfo? FindChild(DirectoryInfo parent, string name) =>
        parent.EnumerateDirectories()
            .FirstOrDefault(child => IsRegularEntry(child)
                && child.Name.Equals(name, StringComparison.Ordinal));

    private static bool IsRegularEntry(FileSystemInfo entry) =>
        (entry.Attributes & FileAttributes.ReparsePoint) == 0;
}
