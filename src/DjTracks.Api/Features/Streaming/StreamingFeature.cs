using DjTracks.Api.Features.Streaming.GetTrackWaveform;
using DjTracks.Api.Features.Streaming.GetTrackArtwork;
using DjTracks.Api.Features.Streaming.StreamTrack;
using DjTracks.Api.Features.Streaming.StreamSession;
using DjTracks.Api.Features.Streaming.GetSessionArtwork;
using DjTracks.Api.Features.Streaming.GetSessionWaveform;
using DjTracks.Infrastructure.Streaming;

namespace DjTracks.Api.Features.Streaming;

public static class StreamingFeature
{
    public static IServiceCollection AddStreamingFeature(this IServiceCollection services) =>
        services.AddScoped<StreamTrackHandler>()
            .AddScoped<StreamSessionHandler>()
            .AddScoped<GetSessionArtworkHandler>()
            .AddScoped<GetSessionWaveformHandler>()
            .AddSingleton<IWaveformGenerator>(sp => new FfmpegWaveformGenerator(
                sp.GetRequiredService<IConfiguration>()["Waveform:BinaryFolder"]))
            .AddSingleton(sp => new TrackWaveformCache(
                Path.Combine(sp.GetRequiredService<IHostEnvironment>().ContentRootPath,
                    "App_Data", "Waveforms"),
                sp.GetRequiredService<IWaveformGenerator>(),
                sp.GetRequiredService<ILogger<TrackWaveformCache>>()))
            .AddScoped<GetTrackWaveformHandler>()
            .AddScoped<GetTrackArtworkHandler>();

    public static RouteGroupBuilder MapStreamingFeature(this RouteGroupBuilder api)
    {
        api.MapStreamTrack();
        api.MapStreamSession();
        api.MapGetSessionArtwork();
        api.MapGetSessionWaveform();
        api.MapGetTrackWaveform();
        api.MapGetTrackArtwork();
        return api;
    }
}
