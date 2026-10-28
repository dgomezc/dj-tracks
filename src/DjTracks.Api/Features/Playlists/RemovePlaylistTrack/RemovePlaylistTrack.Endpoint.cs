using DjTracks.Api.Shared;

namespace DjTracks.Api.Features.Playlists.RemovePlaylistTrack;

public sealed record RemovePlaylistTrackRequest(Guid Id, Guid EntryId);

public static class RemovePlaylistTrackEndpoint
{
    public static void MapRemovePlaylistTrack(this RouteGroupBuilder api) =>
        api.MapDelete("playlists/{id:guid}/tracks/{entryId:guid}", Handle).WithTags("Playlists");

    private static async Task<IResult> Handle([AsParameters] RemovePlaylistTrackRequest request,
        RemovePlaylistTrackHandler handler, CancellationToken ct) =>
        (await handler.HandleAsync(request, ct)).ToHttpResult();
}
