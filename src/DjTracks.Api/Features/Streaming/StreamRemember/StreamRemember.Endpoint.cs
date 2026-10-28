using DjTracks.Api.Shared;

namespace DjTracks.Api.Features.Streaming.StreamRemember;

public sealed record StreamRememberRequest(string? Category, string? FileName);

public static class StreamRememberEndpoint
{
    public static void MapStreamRemember(this RouteGroupBuilder api) =>
        api.MapGet("streaming/remember", Handle).WithTags("Streaming");

    private static async Task<IResult> Handle([AsParameters] StreamRememberRequest request,
        StreamRememberHandler handler, CancellationToken ct)
    {
        var result = await handler.HandleAsync(request, ct);
        return result.IsSuccess
            ? Results.File(result.Value, "audio/mpeg", enableRangeProcessing: true)
            : result.ToHttpResult();
    }
}
