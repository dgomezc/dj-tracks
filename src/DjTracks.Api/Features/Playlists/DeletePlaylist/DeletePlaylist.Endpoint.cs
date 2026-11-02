using DjTracks.Api.Shared;

namespace DjTracks.Api.Features.Playlists.DeletePlaylist;

public sealed record DeletePlaylistRequest(Guid Id);

public static class DeletePlaylistEndpoint
{
    public static void MapDeletePlaylist(this RouteGroupBuilder api) =>
        api.MapDelete("playlists/{id:guid}", Handle).WithTags("Playlists");

    private static async Task<IResult> Handle([AsParameters] DeletePlaylistRequest request,
        DeletePlaylistHandler handler, CancellationToken ct) =>
        (await handler.HandleAsync(request, ct)).ToHttpResult();
}
