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

        return new TrackListing(year, style, [], ReadTracks(directory, ct));
    }

    public Result<FileStream> OpenTrack(string root, string year, string style, string fileName,
        CancellationToken ct)
    {
        var result = ResolveTrackFile(root, year, style, fileName, ct);
        if (result.IsFailed)
            return result.ToResult();

        return new FileStream(result.Value.FullName, FileMode.Open, FileAccess.Read, FileShare.Read,
            64 * 1024, FileOptions.Asynchronous | FileOptions.SequentialScan);
    }

    public Result<TrackArtwork> ReadArtwork(string root, string year, string style, string fileName,
        CancellationToken ct)
    {
        var result = ResolveTrackFile(root, year, style, fileName, ct);
        if (result.IsFailed)
            return result.ToResult();

        return ReadArtwork(result.Value, ct);
    }

    internal static Result<TrackArtwork> ReadArtwork(FileInfo file, CancellationToken ct)
    {
        using var audio = TagLib.File.Create(file.FullName);
        ct.ThrowIfCancellationRequested();
        var picture = audio.Tag.Pictures.FirstOrDefault(item => item.Type == TagLib.PictureType.FrontCover)
            ?? audio.Tag.Pictures.FirstOrDefault();
        if (picture is null)
            return new TrackArtwork(null);

        if (picture.MimeType is not ("image/jpeg" or "image/png" or "image/gif" or "image/webp"))
            return new AppError(ErrorKind.Validation, "Formato de carátula no compatible.",
                "La carátula incrustada debe ser JPEG, PNG, GIF o WebP.");

        return new TrackArtwork($"data:{picture.MimeType};base64,{Convert.ToBase64String(picture.Data.Data)}");
    }

    public Result<FileInfo> ResolveTrackFile(string root, string year, string style, string fileName,
        CancellationToken ct)
    {
        var directory = new DirectoryInfo(root);
        foreach (var name in new[] { year, style })
        {
            var child = FindChild(directory, name);
            if (child is null)
                return new AppError(ErrorKind.NotFound, "Carpeta no encontrada.",
                    "La canción seleccionada ya no está en la biblioteca. Vuelve a cargarla.");
            directory = child;
        }

        return ResolveAudioFile(directory, fileName, ct);
    }

    internal static Result<FileInfo> ResolveAudioFile(DirectoryInfo directory, string fileName,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var file = directory.EnumerateFiles()
            .FirstOrDefault(candidate => IsRegularEntry(candidate)
                && candidate.Name.Equals(fileName, StringComparison.Ordinal));
        if (file is null || !file.Extension.Equals(".mp3", StringComparison.OrdinalIgnoreCase))
            return new AppError(ErrorKind.NotFound, "Canción no encontrada.",
                "El fichero seleccionado ya no existe. Vuelve a cargar la biblioteca.");

        return file;
    }

    public TrackFilterOptions GetFilterOptions(string root, CancellationToken ct)
    {
        var yearDirectories = new DirectoryInfo(root).EnumerateDirectories()
            .Where(IsRegularEntry)
            .ToArray();
        HashSet<string> groups = new(StringComparer.OrdinalIgnoreCase);
        foreach (var year in yearDirectories)
        {
            ct.ThrowIfCancellationRequested();
            foreach (var style in year.EnumerateDirectories().Where(IsRegularEntry))
                groups.Add(SplitStyle(style.Name).Group);
        }
        return new TrackFilterOptions(
            yearDirectories.Select(year => year.Name)
                .OrderByDescending(year => year, StringComparer.OrdinalIgnoreCase).ToArray(),
            groups.OrderBy(group => group, StringComparer.OrdinalIgnoreCase).ToArray());
    }

    public Result<TrackListing> Search(string root, TrackFilter filter, CancellationToken ct)
    {
        var selectedYears = filter.Years.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var selectedGroups = filter.Groups.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var selectedSubstyles = filter.Substyles.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var rootDirectory = new DirectoryInfo(root);
        List<DirectoryInfo> yearDirectories = [];
        if (selectedYears.Count == 0)
        {
            yearDirectories.AddRange(rootDirectory.EnumerateDirectories().Where(IsRegularEntry)
                .OrderByDescending(directory => directory.Name, StringComparer.OrdinalIgnoreCase));
        }
        else
        {
            foreach (var year in selectedYears)
            {
                var yearDirectory = FindChild(rootDirectory, year);
                if (yearDirectory is null)
                    return new AppError(ErrorKind.NotFound, "Año no encontrado.",
                        "Uno de los años seleccionados ya no existe. Vuelve a cargar los filtros.");
                yearDirectories.Add(yearDirectory);
            }
            yearDirectories.Sort((left, right) =>
                StringComparer.OrdinalIgnoreCase.Compare(right.Name, left.Name));
        }

        List<TrackFile> tracks = [];

        foreach (var yearDirectory in yearDirectories)
        {
            ct.ThrowIfCancellationRequested();
            foreach (var styleDirectory in yearDirectory.EnumerateDirectories()
                .Where(IsRegularEntry)
                .OrderBy(directory => directory.Name, StringComparer.OrdinalIgnoreCase))
            {
                ct.ThrowIfCancellationRequested();
                var (group, substyle) = SplitStyle(styleDirectory.Name);
                if ((selectedGroups.Count > 0 && !selectedGroups.Contains(group))
                    || (selectedSubstyles.Count > 0
                        && (substyle is null || !selectedSubstyles.Contains(substyle))))
                    continue;

                tracks.AddRange(ReadTracks(styleDirectory, ct));
            }
        }

        return new TrackListing(null, null, [], tracks.ToArray());
    }

    internal static DirectoryInfo? FindChild(DirectoryInfo parent, string name) =>
        parent.EnumerateDirectories()
            .FirstOrDefault(child => IsRegularEntry(child)
                && child.Name.Equals(name, StringComparison.Ordinal));

    private static bool IsRegularEntry(FileSystemInfo entry) =>
        (entry.Attributes & FileAttributes.ReparsePoint) == 0;

    internal static TrackFile[] ReadTracks(DirectoryInfo directory, CancellationToken ct)
    {
        List<TrackFile> tracks = [];
        foreach (var file in directory.EnumerateFiles()
            .Where(file => IsRegularEntry(file)
                && file.Extension.Equals(".mp3", StringComparison.OrdinalIgnoreCase))
            .OrderBy(file => file.Name, StringComparer.OrdinalIgnoreCase))
        {
            tracks.Add(ReadTrack(file, ct));
        }
        return tracks.ToArray();
    }

    internal static TrackFile ReadTrack(FileInfo file, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        try
        {
            using var audio = TagLib.File.Create(file.FullName);
            return new(file.Name, audio.Tag.Title, audio.Tag.Performers,
                audio.Tag.Album, audio.Properties.Duration.TotalSeconds, null,
                file.Directory?.Parent?.Name, file.Directory?.Name);
        }
        catch (Exception ex) when (ex is TagLib.CorruptFileException or TagLib.UnsupportedFormatException
            or IOException or UnauthorizedAccessException)
        {
            return new(file.Name, null, [], null, null,
                "No se pudieron leer los tags o el audio. Comprueba el fichero y sus permisos.",
                file.Directory?.Parent?.Name, file.Directory?.Name);
        }
    }

    private static (string Group, string? Substyle) SplitStyle(string style)
    {
        foreach (var group in new[] { "Day", "Night" })
        {
            foreach (var substyle in new[] { "Instrumental", "Vocal" })
            {
                if (style.Equals($"{group} {substyle}", StringComparison.OrdinalIgnoreCase)
                    || (group == "Night" && substyle == "Instrumental"
                        && style.Equals("Night Intrumental", StringComparison.OrdinalIgnoreCase)))
                    return (group, substyle);
            }
        }
        return (style, null);
    }
}
