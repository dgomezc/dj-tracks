using DjTracks.Api.Shared;

namespace DjTracks.Api.Features.Streaming.StreamTrack;

public sealed record StreamTrackRequest(string? Year, string? Style, string? FileName);

public static class StreamTrackEndpoint
{
    public static void MapStreamTrack(this RouteGroupBuilder api) =>
        api.MapGet("streaming/tracks", Handle).WithTags("Streaming");

    private static async Task<IResult> Handle([AsParameters] StreamTrackRequest request,
        StreamTrackHandler handler, CancellationToken ct)
    {
        var result = await handler.HandleAsync(request, ct);
        return result.IsSuccess
            ? Results.File(result.Value, "audio/mpeg", enableRangeProcessing: true)
            : result.ToHttpResult();
    }
}
