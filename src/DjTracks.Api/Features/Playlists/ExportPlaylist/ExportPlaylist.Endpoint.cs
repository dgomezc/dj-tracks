using DjTracks.Api.Shared;

namespace DjTracks.Api.Features.Playlists.ExportPlaylist;

public sealed record ExportPlaylistRequest(Guid Id);

public static class ExportPlaylistEndpoint
{
    public static void MapExportPlaylist(this RouteGroupBuilder api) =>
        api.MapPost("playlists/{id:guid}/export", Handle).WithTags("Playlists");

    private static async Task<IResult> Handle([AsParameters] ExportPlaylistRequest request,
        ExportPlaylistHandler handler, CancellationToken ct) =>
        (await handler.HandleAsync(request, ct)).ToHttpResult();
}
