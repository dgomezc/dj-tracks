using DjTracks.Api.Features.Streaming.GetRememberArtwork;
using DjTracks.Api.Features.Streaming.GetSessionArtwork;
using DjTracks.Api.Features.Streaming.GetTrackArtwork;
using DjTracks.Domain;
using FluentResults;

namespace DjTracks.Api.Features.Subsonic.GetCoverArt;

public sealed class GetCoverArtHandler(GetTrackArtworkHandler tracks, GetSessionArtworkHandler sessions,
    GetRememberArtworkHandler remember)
{
    public Result<TrackArtwork> Handle(SubsonicId id, CancellationToken ct) =>
        id.Library switch
        {
            "Tracks" => tracks.Handle(new GetTrackArtworkRequest(id.Parts[0], id.Parts[1], id.Parts[2]), ct),
            "Sessions" => sessions.Handle(new GetSessionArtworkRequest(id.Parts[0], id.Parts[1], id.Parts[2]), ct),
            "Remember" => remember.Handle(new GetRememberArtworkRequest(id.Parts[0], id.Parts[1]), ct),
            _ => throw new InvalidOperationException("Biblioteca Subsonic no válida.")
        };
}
