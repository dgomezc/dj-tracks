using DjTracks.Api.Shared;

namespace DjTracks.Api.Features.Library.GetTracks;

public sealed record GetTracksRequest(string? Year, string? Style);

public static class GetTracksEndpoint
{
    public static void MapGetTracks(this RouteGroupBuilder api) =>
        api.MapGet("library/tracks", Handle).WithTags("Library");

    private static async Task<IResult> Handle([AsParameters] GetTracksRequest request,
        GetTracksHandler handler, CancellationToken ct) =>
        (await handler.HandleAsync(request, ct)).ToHttpResult();
}
