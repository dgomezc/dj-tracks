using DjTracks.Api.Shared;

namespace DjTracks.Api.Features.Streaming.StreamSession;

public sealed record StreamSessionRequest(string? Year, string? Session, string? FileName);

public static class StreamSessionEndpoint
{
    public static void MapStreamSession(this RouteGroupBuilder api) =>
        api.MapGet("streaming/sessions", Handle).WithTags("Streaming");

    private static async Task<IResult> Handle([AsParameters] StreamSessionRequest request,
        StreamSessionHandler handler, CancellationToken ct)
    {
        var result = await handler.HandleAsync(request, ct);
        return result.IsSuccess
            ? Results.File(result.Value, "audio/mpeg", enableRangeProcessing: true)
            : result.ToHttpResult();
    }
}
