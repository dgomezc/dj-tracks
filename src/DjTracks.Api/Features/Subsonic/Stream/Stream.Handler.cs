using DjTracks.Api.Features.Streaming.StreamRemember;
using DjTracks.Api.Features.Streaming.StreamSession;
using DjTracks.Api.Features.Streaming.StreamTrack;
using FluentResults;

namespace DjTracks.Api.Features.Subsonic.Stream;

public sealed class SubsonicStreamHandler(StreamTrackHandler tracks, StreamSessionHandler sessions,
    StreamRememberHandler remember)
{
    public Task<Result<FileStream>> HandleAsync(SubsonicId id, CancellationToken ct) =>
        id.Library switch
        {
            "Tracks" => tracks.HandleAsync(new StreamTrackRequest(id.Parts[0], id.Parts[1], id.Parts[2]), ct),
            "Sessions" => sessions.HandleAsync(new StreamSessionRequest(id.Parts[0], id.Parts[1], id.Parts[2]), ct),
            "Remember" => remember.HandleAsync(new StreamRememberRequest(id.Parts[0], id.Parts[1]), ct),
            _ => throw new InvalidOperationException("Biblioteca Subsonic no válida.")
        };
}
