using DjTracks.Api.Shared;
using DjTracks.Domain;

namespace DjTracks.Api.Features.Stats.RecordPlayback;

public sealed record RecordPlaybackRequest(PlaybackRecord? Playback);

public static class RecordPlaybackEndpoint
{
    public static void MapRecordPlayback(this RouteGroupBuilder api) =>
        api.MapPost("stats/plays", Handle).WithTags("Stats");

    private static async Task<IResult> Handle(RecordPlaybackRequest request,
        RecordPlaybackHandler handler, CancellationToken ct) =>
        (await handler.HandleAsync(request, ct)).ToHttpResult();
}
