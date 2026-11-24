using FluentValidation;

namespace DjTracks.Api.Features.Subsonic.GetPlaylists;

public static class GetPlaylistsEndpoint
{
    public static void MapGetPlaylists(this RouteGroupBuilder rest)
    {
        rest.MapGet("/getPlaylists", List);
        rest.MapGet("/getPlaylists.view", List);
        rest.MapGet("/getPlaylist", Get);
        rest.MapGet("/getPlaylist.view", Get);
    }

    private static async Task<IResult> List(HttpContext context, GetPlaylistsHandler handler,
        CancellationToken ct)
    {
        var result = await handler.HandleAsync(ct);
        return result.IsSuccess
            ? SubsonicResponse.Ok(context, "playlists", new { playlist = result.Value })
            : SubsonicResponse.FromError(context, result);
    }

    private static async Task<IResult> Get(HttpContext context, GetPlaylistsHandler handler,
        IValidator<GetSubsonicPlaylistRequest> validator, CancellationToken ct)
    {
        var request = new GetSubsonicPlaylistRequest(context.Request.Query["id"]);
        if (!(await validator.ValidateAsync(request, ct)).IsValid)
            return SubsonicResponse.Error(context, 70, "Playlist no encontrada.");
        var id = Guid.Parse(request.Id!);
        var result = await handler.GetAsync(id, ct);
        return result.IsSuccess
            ? SubsonicResponse.Ok(context, "playlist", result.Value)
            : SubsonicResponse.FromError(context, result);
    }
}
