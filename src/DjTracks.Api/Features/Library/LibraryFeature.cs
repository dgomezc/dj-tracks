using DjTracks.Api.Features.Library.GetTracks;
using DjTracks.Api.Features.Library.GetTrackFilters;
using DjTracks.Api.Features.Library.GetFilteredTracks;
using DjTracks.Infrastructure.Library;

namespace DjTracks.Api.Features.Library;

public static class LibraryFeature
{
    public static IServiceCollection AddLibraryFeature(this IServiceCollection services) => services
        .AddSingleton<TrackLibrary>()
        .AddScoped<GetTracksHandler>()
        .AddScoped<GetTrackFiltersHandler>()
        .AddScoped<GetFilteredTracksHandler>();

    public static RouteGroupBuilder MapLibraryFeature(this RouteGroupBuilder api)
    {
        api.MapGetTracks();
        api.MapGetTrackFilters();
        api.MapGetFilteredTracks();
        return api;
    }
}
