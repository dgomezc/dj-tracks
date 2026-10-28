using DjTracks.Api.Features.Library.GetTracks;
using DjTracks.Api.Features.Library.GetSessions;
using DjTracks.Api.Features.Library.GetSessionTracklist;
using DjTracks.Api.Features.Library.GetRemember;
using DjTracks.Api.Features.Library.GetTrackFilters;
using DjTracks.Api.Features.Library.GetFilteredTracks;
using DjTracks.Infrastructure.Library;

namespace DjTracks.Api.Features.Library;

public static class LibraryFeature
{
    public static IServiceCollection AddLibraryFeature(this IServiceCollection services) => services
        .AddSingleton<TrackLibrary>()
        .AddSingleton<SessionLibrary>()
        .AddSingleton<RememberLibrary>()
        .AddScoped<GetRememberHandler>()
        .AddScoped<GetSessionsHandler>()
        .AddScoped<GetSessionTracklistHandler>()
        .AddScoped<GetTracksHandler>()
        .AddScoped<GetTrackFiltersHandler>()
        .AddScoped<GetFilteredTracksHandler>();

    public static RouteGroupBuilder MapLibraryFeature(this RouteGroupBuilder api)
    {
        api.MapGetTracks();
        api.MapGetSessions();
        api.MapGetRemember();
        api.MapGetSessionTracklist();
        api.MapGetTrackFilters();
        api.MapGetFilteredTracks();
        return api;
    }
}
