using DjTracks.Api.Shared;

namespace DjTracks.Api.Features.Streaming.GetTrackArtwork;

public sealed record GetTrackArtworkRequest(string? Year, string? Style, string? FileName);

public static class GetTrackArtworkEndpoint
{
    public static void MapGetTrackArtwork(this RouteGroupBuilder api) =>
        api.MapGet("streaming/tracks/artwork", Handle).WithTags("Streaming");

    private static async Task<IResult> Handle([AsParameters] GetTrackArtworkRequest request,
        GetTrackArtworkHandler handler, CancellationToken ct) =>
        (await handler.HandleAsync(request, ct)).ToHttpResult();
}
