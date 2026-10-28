using DjTracks.Api.Shared;

namespace DjTracks.Api.Features.Streaming.GetRememberArtwork;

public sealed record GetRememberArtworkRequest(string? Category, string? FileName);

public static class GetRememberArtworkEndpoint
{
    public static void MapGetRememberArtwork(this RouteGroupBuilder api) =>
        api.MapGet("streaming/remember/artwork", Handle).WithTags("Streaming");

    private static async Task<IResult> Handle([AsParameters] GetRememberArtworkRequest request,
        GetRememberArtworkHandler handler, CancellationToken ct) =>
        (await handler.HandleAsync(request, ct)).ToHttpResult();
}
