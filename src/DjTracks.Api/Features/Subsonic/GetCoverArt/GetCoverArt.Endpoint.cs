using FluentValidation;

namespace DjTracks.Api.Features.Subsonic.GetCoverArt;

public static class GetCoverArtEndpoint
{
    public static void MapGetCoverArt(this RouteGroupBuilder rest)
    {
        rest.MapGet("/getCoverArt", Handle);
        rest.MapGet("/getCoverArt.view", Handle);
    }

    private static async Task<IResult> Handle(HttpContext context, GetCoverArtHandler handler,
        IValidator<GetSubsonicAudioRequest> validator, CancellationToken ct)
    {
        var request = new GetSubsonicAudioRequest(context.Request.Query["id"]);
        if (!(await validator.ValidateAsync(request, ct)).IsValid)
            return SubsonicResponse.Error(context, 70, "Carátula no encontrada.");
        SubsonicId.TryParse(request.Id, out var id);
        var result = handler.Handle(id, ct);
        if (result.IsFailed)
            return SubsonicResponse.FromError(context, result);
        if (result.Value.DataUrl is null)
            return SubsonicResponse.Error(context, 70, "El fichero no tiene carátula.");
        if (!result.Value.TryGetImage(out var bytes, out var contentType))
        {
            context.RequestServices.GetRequiredService<ILoggerFactory>()
                .CreateLogger("Subsonic.CoverArt").LogError("La carátula del fichero {Id} no es válida.", request.Id);
            return SubsonicResponse.Error(context, 0, "La carátula no es válida.");
        }
        context.Response.Headers.CacheControl = "no-store";
        return Results.File(bytes, contentType);
    }
}
