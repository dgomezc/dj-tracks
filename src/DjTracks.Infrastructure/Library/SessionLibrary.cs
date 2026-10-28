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
        return new SessionListing(year, session, folders,
            listing.Tracks.Select(file => file with { Style = null }).ToArray());
    }

    public Result<FileStream> OpenAudio(string root, string year, string session, string fileName,
        CancellationToken ct) => tracks.OpenTrack(root, year, session, fileName, ct);
}
