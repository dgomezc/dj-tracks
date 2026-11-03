using DjTracks.Api.Shared;
using DjTracks.Domain;

namespace DjTracks.Api.Features.Playlists.GenerateAutomaticPlaylist;

public sealed record GenerateAutomaticPlaylistRequest(string? Name, AutomaticPlaylistFilters? Filters);

public static class GenerateAutomaticPlaylistEndpoint
{
    public static void MapGenerateAutomaticPlaylist(this RouteGroupBuilder api)
    {
        api.MapPost("playlists/automatic", HandleCreate).WithTags("Playlists");
        api.MapPut("playlists/{id:guid}/automatic", HandleUpdate).WithTags("Playlists");
    }

    private static async Task<IResult> HandleCreate(GenerateAutomaticPlaylistRequest request,
        GenerateAutomaticPlaylistHandler handler, CancellationToken ct) =>
        (await handler.HandleAsync(null, request, ct)).ToHttpResult();

    private static async Task<IResult> HandleUpdate(Guid id, GenerateAutomaticPlaylistRequest request,
        GenerateAutomaticPlaylistHandler handler, CancellationToken ct) =>
        (await handler.HandleAsync(id, request, ct)).ToHttpResult();
}
