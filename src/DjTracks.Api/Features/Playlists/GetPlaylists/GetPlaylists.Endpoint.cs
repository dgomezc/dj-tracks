using DjTracks.Api.Shared;

namespace DjTracks.Api.Features.Playlists.GetPlaylists;

public static class GetPlaylistsEndpoint
{
    public static void MapGetPlaylists(this RouteGroupBuilder api) =>
        api.MapGet("playlists", Handle).WithTags("Playlists");

    private static async Task<IResult> Handle(GetPlaylistsHandler handler, CancellationToken ct) =>
        (await handler.HandleAsync(ct)).ToHttpResult();
}
