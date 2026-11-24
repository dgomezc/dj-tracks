namespace DjTracks.Api.Features.Subsonic.GetIndexes;

public static class GetIndexesEndpoint
{
    public static void MapGetIndexes(this RouteGroupBuilder rest)
    {
        rest.MapGet("/getIndexes", Handle);
        rest.MapGet("/getIndexes.view", Handle);
    }

    private static IResult Handle(HttpContext context, GetIndexesHandler handler,
        CancellationToken ct)
    {
        var folder = context.Request.Query["musicFolderId"].ToString();
        var result = handler.Handle(string.IsNullOrEmpty(folder) ? null : folder, ct);
        return result.IsSuccess
            ? SubsonicResponse.Ok(context, "indexes", result.Value)
            : SubsonicResponse.FromError(context, result);
    }
}
