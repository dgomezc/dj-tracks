using DjTracks.Api.Shared;

namespace DjTracks.Api.Features.Library.GetTracks;

public sealed record GetTracksRequest(string? Year, string? Style);

public static class GetTracksEndpoint
{
    public static void MapGetTracks(this RouteGroupBuilder api) =>
        api.MapGet("library/tracks", Handle).WithTags("Library");

    private static IResult Handle([AsParameters] GetTracksRequest request,
        GetTracksHandler handler, CancellationToken ct) =>
        handler.Handle(request, ct).ToHttpResult();
}
