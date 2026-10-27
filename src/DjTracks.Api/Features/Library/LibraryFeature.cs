using DjTracks.Api.Features.Library.GetTracks;
using DjTracks.Infrastructure.Library;

namespace DjTracks.Api.Features.Library;

public static class LibraryFeature
{
    public static IServiceCollection AddLibraryFeature(this IServiceCollection services) => services
        .AddSingleton<TrackLibrary>()
        .AddScoped<GetTracksHandler>();

    public static RouteGroupBuilder MapLibraryFeature(this RouteGroupBuilder api)
    {
        api.MapGetTracks();
        return api;
    }
}
