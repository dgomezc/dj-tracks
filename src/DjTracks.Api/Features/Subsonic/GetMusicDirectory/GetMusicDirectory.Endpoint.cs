using FluentValidation;

namespace DjTracks.Api.Features.Subsonic.GetMusicDirectory;

public static class GetMusicDirectoryEndpoint
{
    public static void MapGetMusicDirectory(this RouteGroupBuilder rest)
    {
        rest.MapGet("/getMusicDirectory", Handle);
        rest.MapGet("/getMusicDirectory.view", Handle);
    }

    private static async Task<IResult> Handle(HttpContext context, GetMusicDirectoryHandler handler,
        IValidator<GetSubsonicDirectoryRequest> validator, CancellationToken ct)
    {
        var request = new GetSubsonicDirectoryRequest(context.Request.Query["id"]);
        if (!(await validator.ValidateAsync(request, ct)).IsValid)
            return SubsonicResponse.Error(context, 70, "Carpeta no encontrada.");
        SubsonicId.TryParse(request.Id, out var id);
        var result = handler.Handle(id, ct);
        return result.IsSuccess
            ? SubsonicResponse.Ok(context, "directory", result.Value)
            : SubsonicResponse.FromError(context, result);
    }
}
