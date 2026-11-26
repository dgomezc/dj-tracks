using DjTracks.Domain;
using Microsoft.AspNetCore.WebUtilities;

namespace DjTracks.UI.Shared;

public static class ArtworkEndpoint
{
    public static void MapArtworkEndpoint(this WebApplication app) =>
        app.MapGet("/artwork/{library}", Handle);

    private static async Task<IResult> Handle(string library, HttpContext context,
        IHttpClientFactory factory, ILoggerFactory loggerFactory, CancellationToken ct)
    {
        var path = library switch
        {
            "tracks" => "api/streaming/tracks/artwork",
            "sessions" => "api/streaming/sessions/artwork",
            "remember" => "api/streaming/remember/artwork",
            _ => null
        };
        if (path is null)
            return Results.NotFound();

        var query = new Dictionary<string, string?>
        {
            ["year"] = context.Request.Query["year"].ToString(),
            ["style"] = context.Request.Query["style"].ToString(),
            ["session"] = context.Request.Query["session"].ToString(),
            ["category"] = context.Request.Query["category"].ToString(),
            ["fileName"] = context.Request.Query["fileName"].ToString()
        };
        var logger = loggerFactory.CreateLogger("Artwork");
        var result = await factory.CreateClient("Api").GetForResultAsync<TrackArtwork>(
            QueryHelpers.AddQueryString(path, query), logger, ct);
        if (result.IsFailed)
            return Results.Problem(result.Errors[0].Message, statusCode: StatusCodes.Status503ServiceUnavailable);
        if (result.Value.DataUrl is null)
            return Results.NotFound();
        if (!result.Value.TryGetImage(out var bytes, out var contentType))
        {
            logger.LogError("La API devolvió una carátula no válida para {Library}.", library);
            return Results.Problem("La carátula no es válida.");
        }
        context.Response.Headers.CacheControl = "no-store";
        return Results.File(bytes, contentType);
    }
}
