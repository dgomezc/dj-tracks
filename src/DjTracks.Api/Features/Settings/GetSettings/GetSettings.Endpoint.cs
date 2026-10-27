using DjTracks.Api.Shared;

namespace DjTracks.Api.Features.Settings.GetSettings;

public static class GetSettingsEndpoint
{
    public static void MapGetSettings(this RouteGroupBuilder api) =>
        api.MapGet("settings", Handle).WithTags("Settings");

    private static async Task<IResult> Handle(GetSettingsHandler handler, CancellationToken ct) =>
        (await handler.HandleAsync(ct)).ToHttpResult();
}
