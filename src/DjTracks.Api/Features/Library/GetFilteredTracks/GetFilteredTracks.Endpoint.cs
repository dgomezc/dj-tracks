using DjTracks.Api.Shared;

namespace DjTracks.Api.Features.Library.GetFilteredTracks;

public sealed record GetFilteredTracksRequest(string[]? Year, string[]? Group, string[]? Substyle);

public static class GetFilteredTracksEndpoint
{
    public static void MapGetFilteredTracks(this RouteGroupBuilder api) =>
        api.MapGet("library/tracks/search", Handle).WithTags("Library");

    private static IResult Handle([AsParameters] GetFilteredTracksRequest request,
        GetFilteredTracksHandler handler, CancellationToken ct) =>
        handler.Handle(request, ct).ToHttpResult();
}
