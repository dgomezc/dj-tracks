using DjTracks.Api.Shared;

namespace DjTracks.Api.Features.Playlists.RenamePlaylist;

public sealed record RenamePlaylistRequest(string? Name);

public static class RenamePlaylistEndpoint
{
    public static void MapRenamePlaylist(this RouteGroupBuilder api) =>
        api.MapPut("playlists/{id:guid}", Handle).WithTags("Playlists");

    private static async Task<IResult> Handle(Guid id, RenamePlaylistRequest request,
        RenamePlaylistHandler handler, CancellationToken ct) =>
        (await handler.HandleAsync(id, request, ct)).ToHttpResult();
}
