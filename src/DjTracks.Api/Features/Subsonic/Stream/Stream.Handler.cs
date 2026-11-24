using DjTracks.Api.Features.Streaming.StreamRemember;
using DjTracks.Api.Features.Streaming.StreamSession;
using DjTracks.Api.Features.Streaming.StreamTrack;
using FluentResults;

namespace DjTracks.Api.Features.Subsonic.Stream;

public sealed class SubsonicStreamHandler(StreamTrackHandler tracks, StreamSessionHandler sessions,
    StreamRememberHandler remember)
{
    public Result<FileStream> Handle(SubsonicId id, CancellationToken ct) =>
        id.Library switch
        {
            "Tracks" => tracks.Handle(new StreamTrackRequest(id.Parts[0], id.Parts[1], id.Parts[2]), ct),
            "Sessions" => sessions.Handle(new StreamSessionRequest(id.Parts[0], id.Parts[1], id.Parts[2]), ct),
            "Remember" => remember.Handle(new StreamRememberRequest(id.Parts[0], id.Parts[1]), ct),
            _ => throw new InvalidOperationException("Biblioteca Subsonic no válida.")
        };
}
