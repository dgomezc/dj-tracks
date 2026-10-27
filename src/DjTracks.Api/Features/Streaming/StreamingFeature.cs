using DjTracks.Api.Features.Streaming.StreamTrack;

namespace DjTracks.Api.Features.Streaming;

public static class StreamingFeature
{
    public static IServiceCollection AddStreamingFeature(this IServiceCollection services) =>
        services.AddScoped<StreamTrackHandler>();

    public static RouteGroupBuilder MapStreamingFeature(this RouteGroupBuilder api)
    {
        api.MapStreamTrack();
        return api;
    }
}
