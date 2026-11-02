using DjTracks.Api.Shared;

namespace DjTracks.Api.Features.Playlists.CreatePlaylist;

public sealed record CreatePlaylistRequest(string? Name);

public static class CreatePlaylistEndpoint
{
    public static void MapCreatePlaylist(this RouteGroupBuilder api) =>
        api.MapPost("playlists", Handle).WithTags("Playlists");

    private static async Task<IResult> Handle(CreatePlaylistRequest request,
        CreatePlaylistHandler handler, CancellationToken ct) =>
        (await handler.HandleAsync(request, ct)).ToHttpResult();
}
