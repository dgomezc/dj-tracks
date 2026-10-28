using DjTracks.Domain;
using FluentResults;

namespace DjTracks.Infrastructure.Library;

public sealed class SessionLibrary(TrackLibrary tracks)
{
    public Result<SessionListing> Browse(string root, string? year, string? session, CancellationToken ct)
    {
        // Ambas bibliotecas tienen dos niveles de carpetas antes de los MP3.
        var result = tracks.Browse(root, year, session, ct);
        if (result.IsFailed)
            return result.ToResult();

        var listing = result.Value;
        var folders = listing.Folders;
        if (year is not null && session is null)
            Array.Reverse(folders);

        string[] tracklists = [];
        if (year is not null && session is not null)
        {
            var yearDirectory = TrackLibrary.FindChild(new DirectoryInfo(root), year);
            var directory = yearDirectory is null ? null : TrackLibrary.FindChild(yearDirectory, session);
            if (directory is null)
                return new AppError(ErrorKind.NotFound, "Carpeta no encontrada.",
                    "La carpeta seleccionada ya no existe. Vuelve al nivel anterior.");

            HashSet<string> names = new(StringComparer.OrdinalIgnoreCase) { "Tracklist" };
            foreach (var name in listing.Tracks.Select(file => Path.GetFileNameWithoutExtension(file.FileName))
                .Append(session))
            {
                names.Add(name);
                names.Add($"Tracklist - {name}");
                names.Add($"{name} - Tracklist");
            }

            List<string> candidates = [];
            foreach (var file in directory.EnumerateFiles())
            {
                ct.ThrowIfCancellationRequested();
                if ((file.Attributes & FileAttributes.ReparsePoint) == 0
                    && file.Extension.Equals(".txt", StringComparison.OrdinalIgnoreCase)
                    && names.Contains(Path.GetFileNameWithoutExtension(file.Name)))
                    candidates.Add(file.Name);
            }
            tracklists = candidates.OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                .ThenBy(name => name, StringComparer.Ordinal).ToArray();
        }

        return new SessionListing(year, session, folders,
            listing.Tracks.Select(file => file with { Style = null }).ToArray(), tracklists);
    }

    public Result<FileStream> OpenAudio(string root, string year, string session, string fileName,
        CancellationToken ct) => tracks.OpenTrack(root, year, session, fileName, ct);
}
