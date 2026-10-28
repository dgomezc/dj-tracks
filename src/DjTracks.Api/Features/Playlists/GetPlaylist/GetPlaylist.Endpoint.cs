using DjTracks.Api.Shared;

namespace DjTracks.Api.Features.Playlists.GetPlaylist;

public sealed record GetPlaylistRequest(Guid Id);

public static class GetPlaylistEndpoint
{
    public static void MapGetPlaylist(this RouteGroupBuilder api) =>
        api.MapGet("playlists/{id:guid}", Handle).WithTags("Playlists");

    private static async Task<IResult> Handle([AsParameters] GetPlaylistRequest request,
        GetPlaylistHandler handler, CancellationToken ct) =>
        (await handler.HandleAsync(request, ct)).ToHttpResult();
}
