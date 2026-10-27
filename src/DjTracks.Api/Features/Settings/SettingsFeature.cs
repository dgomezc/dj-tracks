using DjTracks.Api.Features.Settings.GetSettings;
using DjTracks.Api.Features.Settings.SaveSettings;
using DjTracks.Infrastructure.Settings;

namespace DjTracks.Api.Features.Settings;

public static class SettingsFeature
{
    public static IServiceCollection AddSettingsFeature(this IServiceCollection services) => services
        .AddSingleton(sp => new JsonSettingsStore(Path.GetFullPath(
                sp.GetRequiredService<IConfiguration>()["Settings:FilePath"] ?? Path.Combine("App_Data", "settings.json"),
                sp.GetRequiredService<IHostEnvironment>().ContentRootPath),
            sp.GetRequiredService<ILogger<JsonSettingsStore>>()))
        .AddScoped<GetSettingsHandler>()
        .AddScoped<SaveSettingsHandler>();

    public static RouteGroupBuilder MapSettingsFeature(this RouteGroupBuilder api)
    {
        api.MapGetSettings();
        api.MapSaveSettings();
        return api;
    }
}
