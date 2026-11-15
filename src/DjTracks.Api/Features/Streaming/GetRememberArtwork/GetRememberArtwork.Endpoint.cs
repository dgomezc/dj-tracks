using DjTracks.Api.Shared;

namespace DjTracks.Api.Features.Streaming.GetRememberArtwork;

public sealed record GetRememberArtworkRequest(string? Category, string? FileName);

public static class GetRememberArtworkEndpoint
{
    public static void MapGetRememberArtwork(this RouteGroupBuilder api) =>
        api.MapGet("streaming/remember/artwork", Handle).WithTags("Streaming");

    private static IResult Handle([AsParameters] GetRememberArtworkRequest request,
        GetRememberArtworkHandler handler, CancellationToken ct) =>
        handler.Handle(request, ct).ToHttpResult();
}
