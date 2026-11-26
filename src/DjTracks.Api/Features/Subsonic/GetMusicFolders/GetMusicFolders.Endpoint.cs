namespace DjTracks.Api.Features.Subsonic.GetMusicFolders;

public static class GetMusicFoldersEndpoint
{
    public static void MapGetMusicFolders(this RouteGroupBuilder rest)
    {
        rest.MapGet("/getMusicFolders", Handle);
        rest.MapGet("/getMusicFolders.view", Handle);
    }

    private static IResult Handle(HttpContext context, GetMusicFoldersHandler handler)
    {
        var result = handler.Handle();
        return result.IsSuccess
            ? SubsonicResponse.Ok(context, "musicFolders", new { musicFolder = result.Value })
            : SubsonicResponse.FromError(context, result);
    }
}
