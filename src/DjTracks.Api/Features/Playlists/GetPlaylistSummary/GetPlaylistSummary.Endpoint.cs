using DjTracks.Api.Shared;

namespace DjTracks.Api.Features.Playlists.GetPlaylistSummary;

public static class GetPlaylistSummaryEndpoint
{
    public static void MapGetPlaylistSummary(this RouteGroupBuilder api) =>
        api.MapGet("playlists/summary", Handle).WithTags("Playlists");

    private static async Task<IResult> Handle(GetPlaylistSummaryHandler handler, CancellationToken ct) =>
        (await handler.HandleAsync(ct)).ToHttpResult();
}
