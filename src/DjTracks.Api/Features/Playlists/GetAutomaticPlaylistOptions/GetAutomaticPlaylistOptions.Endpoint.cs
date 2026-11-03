using DjTracks.Api.Shared;

namespace DjTracks.Api.Features.Playlists.GetAutomaticPlaylistOptions;

public static class GetAutomaticPlaylistOptionsEndpoint
{
    public static void MapGetAutomaticPlaylistOptions(this RouteGroupBuilder api) =>
        api.MapGet("playlists/automatic/options", Handle).WithTags("Playlists");

    private static async Task<IResult> Handle(GetAutomaticPlaylistOptionsHandler handler, CancellationToken ct) =>
        (await handler.HandleAsync(ct)).ToHttpResult();
}
