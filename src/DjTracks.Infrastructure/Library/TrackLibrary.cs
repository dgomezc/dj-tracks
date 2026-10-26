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

        return new(year, style, [], ReadTracks(directory, ct));
    }

    public TrackFilterOptions GetFilterOptions(string root, CancellationToken ct)
    {
        var years = new DirectoryInfo(root).EnumerateDirectories().Where(IsRegularEntry)
            .OrderByDescending(folder => folder.Name, StringComparer.OrdinalIgnoreCase).ToArray();
        HashSet<string> groups = new(StringComparer.OrdinalIgnoreCase);
        foreach (var year in years)
        {
            ct.ThrowIfCancellationRequested();
            foreach (var style in year.EnumerateDirectories().Where(IsRegularEntry))
                groups.Add(SplitStyle(style.Name).Group);
        }
        return new(years.Select(year => year.Name).ToArray(),
            groups.OrderBy(group => group, StringComparer.OrdinalIgnoreCase).ToArray());
    }

    public TrackListing Search(string root, TrackFilter filter, CancellationToken ct)
    {
        foreach (var substyle in filter.Substyles)
            if (substyle is not ("Instrumental" or "Vocal"))
                throw new ArgumentException("El subestilo debe ser Instrumental o Vocal.");
        foreach (var group in filter.Groups)
            ValidateFolderName(group);

        var directory = new DirectoryInfo(root);
        var years = filter.Years.Length == 0
            ? directory.EnumerateDirectories().Where(IsRegularEntry)
                .OrderByDescending(year => year.Name, StringComparer.OrdinalIgnoreCase)
            : filter.Years.Distinct(StringComparer.Ordinal).Select(year => FindChild(directory, year))
                .OrderByDescending(year => year.Name, StringComparer.OrdinalIgnoreCase);
        List<TrackFile> tracks = [];
        foreach (var year in years)
        {
            ct.ThrowIfCancellationRequested();
            foreach (var style in year.EnumerateDirectories().Where(IsRegularEntry)
                .OrderBy(style => style.Name, StringComparer.OrdinalIgnoreCase))
            {
                ct.ThrowIfCancellationRequested();
                var parts = SplitStyle(style.Name);
                if ((filter.Groups.Length == 0 || filter.Groups.Contains(parts.Group, StringComparer.OrdinalIgnoreCase))
                    && (filter.Substyles.Length == 0
                        || (parts.Substyle is not null && filter.Substyles.Contains(parts.Substyle))))
                    tracks.AddRange(ReadTracks(style, ct));
            }
        }
        return new(null, null, [], tracks.ToArray());
    }

    private static (string Group, string? Substyle) SplitStyle(string name)
    {
        foreach (var group in new[] { "Day", "Night" })
        {
            foreach (var substyle in new[] { "Instrumental", "Vocal" })
            {
                if (name.Equals($"{group} {substyle}", StringComparison.OrdinalIgnoreCase)
                    || (group == "Night" && substyle == "Instrumental"
                        && name.Equals("Night Intrumental", StringComparison.OrdinalIgnoreCase)))
                    return (group, substyle);
            }
        }
        return (name, null);
    }

    private static TrackFile[] ReadTracks(DirectoryInfo directory, CancellationToken ct)
    {
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
                    audio.Tag.Album, audio.Properties.Duration.TotalSeconds, null,
                    directory.Parent!.Name, directory.Name));
            }
            catch (Exception ex) when (ex is TagLib.CorruptFileException or TagLib.UnsupportedFormatException
                or IOException or UnauthorizedAccessException)
            {
                tracks.Add(new(file.Name, null, [], null, null,
                    "No se pudieron leer los tags o el audio. Comprueba el fichero y sus permisos.",
                    directory.Parent!.Name, directory.Name));
            }
        }
        return tracks.ToArray();
    }

    private static DirectoryInfo FindChild(DirectoryInfo parent, string name)
    {
        ValidateFolderName(name);

        return parent.EnumerateDirectories()
            .FirstOrDefault(child => IsRegularEntry(child)
                && child.Name.Equals(name, StringComparison.Ordinal))
            ?? throw new KeyNotFoundException("La carpeta seleccionada ya no existe. Vuelve al nivel anterior.");
    }

    private static void ValidateFolderName(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name is "." or ".."
            || name.IndexOfAny(['/', '\\', ':']) >= 0 || Path.IsPathRooted(name))
            throw new ArgumentException("El año y el estilo deben ser nombres de carpeta, no rutas.");
    }

    private static bool IsRegularEntry(FileSystemInfo entry) =>
        (entry.Attributes & FileAttributes.ReparsePoint) == 0;
}
