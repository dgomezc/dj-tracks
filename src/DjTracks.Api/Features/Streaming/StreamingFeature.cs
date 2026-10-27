using DjTracks.Api.Features.Streaming.GetTrackWaveform;
using DjTracks.Api.Features.Streaming.StreamTrack;
using DjTracks.Infrastructure.Streaming;

namespace DjTracks.Api.Features.Streaming;

public static class StreamingFeature
{
    public static IServiceCollection AddStreamingFeature(this IServiceCollection services) =>
        services.AddScoped<StreamTrackHandler>()
            .AddSingleton<IWaveformGenerator>(sp => new FfmpegWaveformGenerator(
                sp.GetRequiredService<IConfiguration>()["Waveform:BinaryFolder"]))
            .AddSingleton(sp => new TrackWaveformCache(
                Path.Combine(sp.GetRequiredService<IHostEnvironment>().ContentRootPath,
                    "App_Data", "Waveforms"),
                sp.GetRequiredService<IWaveformGenerator>(),
                sp.GetRequiredService<ILogger<TrackWaveformCache>>()))
            .AddScoped<GetTrackWaveformHandler>();

    public static RouteGroupBuilder MapStreamingFeature(this RouteGroupBuilder api)
    {
        api.MapStreamTrack();
        api.MapGetTrackWaveform();
        return api;
    }
}
