using DjTracks.Api.Shared;

namespace DjTracks.Api.Features.Streaming.GetSessionArtwork;

public sealed record GetSessionArtworkRequest(string? Year, string? Session, string? FileName);

public static class GetSessionArtworkEndpoint
{
    public static void MapGetSessionArtwork(this RouteGroupBuilder api) =>
        api.MapGet("streaming/sessions/artwork", Handle).WithTags("Streaming");

    private static async Task<IResult> Handle([AsParameters] GetSessionArtworkRequest request,
        GetSessionArtworkHandler handler, CancellationToken ct) =>
        (await handler.HandleAsync(request, ct)).ToHttpResult();
}
