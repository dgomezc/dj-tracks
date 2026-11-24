using FluentResults;

namespace DjTracks.Api.Features.Subsonic.GetMusicFolders;

public sealed class GetMusicFoldersHandler
{
    public Result<SubsonicFolder[]> Handle()
    {
        return new[] { "Tracks", "Sessions", "Remember" }.Select(name =>
            new SubsonicFolder(new SubsonicId(name, []).Encode(), name)).ToArray();
    }
}

public sealed record SubsonicFolder(string Id, string Name);
