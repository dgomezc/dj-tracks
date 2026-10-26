using DjTracks.Domain;

namespace DjTracks.Infrastructure.Library;

public sealed class TrackLibrary
{
    public TrackListing Browse(string root, string? year, string? style, CancellationToken ct)
    {
        if (style is not null && year is null)
            throw new ArgumentException("Selecciona un año antes del estilo.");
        var directory = new DirectoryInfo(root);
        if (year is not null)
            directory = FindChild(directory, year);
        if (style is not null)
            directory = FindChild(directory, style);

        ct.ThrowIfCancellationRequested();
        if (style is null)
        {
            var folders = directory.EnumerateDirectories()
                .Where(IsRegularEntry)
                .Select(folder => folder.Name)
                .OrderBy(name => name, StringComparer.OrdinalIgnoreCase).ToArray();
            if (year is null)
                Array.Reverse(folders);
            return new(year, style, folders, []);
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
        return new(year, style, [], tracks.ToArray());
    }

    private static DirectoryInfo FindChild(DirectoryInfo parent, string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name is "." or ".."
            || name.IndexOfAny(['/', '\\', ':']) >= 0 || Path.IsPathRooted(name))
            throw new ArgumentException("El año y el estilo deben ser nombres de carpeta, no rutas.");

        return parent.EnumerateDirectories()
            .FirstOrDefault(child => IsRegularEntry(child)
                && child.Name.Equals(name, StringComparison.Ordinal))
            ?? throw new KeyNotFoundException("La carpeta seleccionada ya no existe. Vuelve al nivel anterior.");
    }

    private static bool IsRegularEntry(FileSystemInfo entry) =>
        (entry.Attributes & FileAttributes.ReparsePoint) == 0;
}
