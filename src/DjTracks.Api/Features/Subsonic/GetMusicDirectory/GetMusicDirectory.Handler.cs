using DjTracks.Api.Features.Library.GetRemember;
using DjTracks.Api.Features.Library.GetSessions;
using DjTracks.Api.Features.Library.GetTracks;
using DjTracks.Domain;
using FluentResults;

namespace DjTracks.Api.Features.Subsonic.GetMusicDirectory;

public sealed class GetMusicDirectoryHandler(GetTracksHandler tracks, GetSessionsHandler sessions,
    GetRememberHandler remember)
{
    public Result<SubsonicDirectory> Handle(SubsonicId id, CancellationToken ct)
    {
        if (!id.IsDirectory)
            return new AppError(ErrorKind.NotFound, "Carpeta no encontrada.", "El identificador no es una carpeta.");

        string[] folders = [];
        TrackFile[] files = [];
        if (id.Library == "Tracks")
        {
            var result = tracks.Handle(new GetTracksRequest(
                id.Parts.ElementAtOrDefault(0), id.Parts.ElementAtOrDefault(1)), ct);
            if (result.IsFailed)
                return result.ToResult();
            folders = result.Value.Folders;
            files = result.Value.Tracks;
        }
        else if (id.Library == "Sessions")
        {
            var result = sessions.Handle(new GetSessionsRequest(
                id.Parts.ElementAtOrDefault(0), id.Parts.ElementAtOrDefault(1)), ct);
            if (result.IsFailed)
                return result.ToResult();
            folders = result.Value.Folders;
            files = result.Value.Files;
        }
        else if (id.Parts.Length == 0)
        {
            foreach (var category in new[] { "Cortas", "Largas" })
            {
                var result = remember.Handle(new GetRememberRequest(category), ct);
                if (result.IsFailed)
                    return result.ToResult();
            }
            folders = ["Cortas", "Largas"];
        }
        else
        {
            var result = remember.Handle(new GetRememberRequest(id.Parts[0]), ct);
            if (result.IsFailed)
                return result.ToResult();
            files = result.Value.Files;
        }

        if (folders.Any(folder => !PlaylistTrackReference.IsSegment(folder))
            || files.Any(file => !PlaylistTrackReference.IsSegment(file.FileName)))
            return new AppError(ErrorKind.Validation, "Nombre de fichero no compatible.",
                "La carpeta contiene un nombre que no se puede usar como identificador de Subsonic.");
        var children = folders
            .Select(name => SubsonicChild.Folder(id.Child(name), id))
            .Concat(files.Select(file => SubsonicChild.Song(id.Child(file.FileName), id, file)))
            .ToArray();
        return new SubsonicDirectory(id.Encode(), id.Parts.LastOrDefault() ?? id.Library,
            id.Parts.Length > 0 ? id.Parent().Encode() : null, children);
    }
}

public sealed record SubsonicDirectory(string Id, string Name, string? Parent, SubsonicChild[] Child);

public sealed record SubsonicChild(string Id, string Parent, bool IsDir, string Title,
    string? Artist = null, string? Album = null, int? Duration = null,
    string? ContentType = null, string? Suffix = null, string? Type = null, string? CoverArt = null)
{
    public static SubsonicChild Folder(SubsonicId id, SubsonicId parent) =>
        new(id.Encode(), parent.Encode(), true, id.Parts[^1]);

    public static SubsonicChild Song(SubsonicId id, SubsonicId parent, TrackFile file) =>
        new(id.Encode(), parent.Encode(), false,
            string.IsNullOrWhiteSpace(file.Title) ? Path.GetFileNameWithoutExtension(file.FileName) : file.Title,
            file.Artists.FirstOrDefault(), file.Album,
            file.DurationSeconds is { } duration && double.IsFinite(duration) && duration >= 0
                ? (int)Math.Min(int.MaxValue, duration) : null,
            "audio/mpeg", "mp3", "music", id.Encode());
}
