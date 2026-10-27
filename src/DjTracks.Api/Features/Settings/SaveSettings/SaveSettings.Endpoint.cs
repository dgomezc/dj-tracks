using DjTracks.Api.Shared;

namespace DjTracks.Api.Features.Settings.SaveSettings;

public sealed record SaveSettingsRequest(string? Tracks, string? Sessions, string? Remember,
    string? Playlists, string? M3uRoot);

public static class SaveSettingsEndpoint
{
    public static void MapSaveSettings(this RouteGroupBuilder api) =>
        api.MapPut("settings", Handle).WithTags("Settings");

    private static async Task<IResult> Handle(SaveSettingsRequest request, SaveSettingsHandler handler,
        CancellationToken ct) =>
        (await handler.HandleAsync(request, ct)).ToHttpResult();
}
