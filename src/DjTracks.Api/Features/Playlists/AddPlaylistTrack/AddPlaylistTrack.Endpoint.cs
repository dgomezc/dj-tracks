using DjTracks.Api.Shared;
using DjTracks.Domain;

namespace DjTracks.Api.Features.Playlists.AddPlaylistTrack;

public sealed record AddPlaylistTrackRequest(PlaylistTrackReference? Track);

public static class AddPlaylistTrackEndpoint
{
    public static void MapAddPlaylistTrack(this RouteGroupBuilder api) =>
        api.MapPost("playlists/{id:guid}/tracks", Handle).WithTags("Playlists");

    private static async Task<IResult> Handle(Guid id, AddPlaylistTrackRequest request,
        AddPlaylistTrackHandler handler, CancellationToken ct) =>
        (await handler.HandleAsync(id, request, ct)).ToHttpResult();
}
