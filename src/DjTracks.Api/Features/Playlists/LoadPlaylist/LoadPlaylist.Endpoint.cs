using DjTracks.Api.Shared;

namespace DjTracks.Api.Features.Playlists.LoadPlaylist;

public sealed record LoadPlaylistRequest(Guid Id);

public static class LoadPlaylistEndpoint
{
    public static void MapLoadPlaylist(this RouteGroupBuilder api) =>
        api.MapGet("playlists/{id:guid}/playback", Handle).WithTags("Playlists");

    private static async Task<IResult> Handle([AsParameters] LoadPlaylistRequest request,
        LoadPlaylistHandler handler, CancellationToken ct) =>
        (await handler.HandleAsync(request, ct)).ToHttpResult();
}
