using DjTracks.Domain;

namespace DjTracks.Infrastructure.Settings;

public static class FolderBrowser
{
    public static FolderListing Browse(string? path, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return new FolderListing(null, null, OperatingSystem.IsWindows()
                ? Directory.GetLogicalDrives()
                : [Path.DirectorySeparatorChar.ToString()]);
        }

        if (!Path.IsPathFullyQualified(path))
            throw new ArgumentException("La ruta debe ser absoluta.", nameof(path));

        var currentPath = Path.TrimEndingDirectorySeparator(Path.GetFullPath(path));
        List<string> directories = [];
        foreach (var directory in Directory.EnumerateDirectories(currentPath))
        {
            cancellationToken.ThrowIfCancellationRequested();
            directories.Add(directory);
        }
        directories.Sort(OperatingSystem.IsWindows() ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);

        return new FolderListing(currentPath, Directory.GetParent(currentPath)?.FullName, directories.ToArray());
    }
}
