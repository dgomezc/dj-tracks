using DjTracks.Api.Features.Playlists.AddPlaylistTrack;
using DjTracks.Api.Features.Playlists.CreatePlaylist;
using DjTracks.Api.Features.Playlists.DeletePlaylist;
using DjTracks.Api.Features.Playlists.GetPlaylist;
using DjTracks.Api.Features.Playlists.GetPlaylists;
using DjTracks.Api.Features.Playlists.GetPlaylistSummary;
using DjTracks.Api.Features.Playlists.LoadPlaylist;
using DjTracks.Api.Features.Playlists.RemovePlaylistTrack;
using DjTracks.Api.Features.Playlists.RenamePlaylist;
using DjTracks.Api.Features.Playlists.GenerateAutomaticPlaylist;
using DjTracks.Api.Features.Playlists.GetAutomaticPlaylistOptions;
using DjTracks.Infrastructure.Playlists;
using DjTracks.Infrastructure.Library;

namespace DjTracks.Api.Features.Playlists;

public static class PlaylistsFeature
{
    public static IServiceCollection AddPlaylistsFeature(this IServiceCollection services) =>
        services.AddSingleton<M3uPlaylistStore>()
            .AddSingleton<PlaylistLibraryIndex>()
            .AddHostedService<PlaylistLibraryIndexWorker>()
            .AddScoped<PlaylistTrackResolver>()
            .AddScoped<GetPlaylistsHandler>().AddScoped<GetPlaylistHandler>()
            .AddScoped<GetPlaylistSummaryHandler>()
            .AddScoped<CreatePlaylistHandler>().AddScoped<RenamePlaylistHandler>()
            .AddScoped<DeletePlaylistHandler>().AddScoped<AddPlaylistTrackHandler>()
            .AddScoped<RemovePlaylistTrackHandler>().AddScoped<LoadPlaylistHandler>()
            .AddScoped<GenerateAutomaticPlaylistHandler>().AddScoped<GetAutomaticPlaylistOptionsHandler>();

    public static void MapPlaylistsFeature(this RouteGroupBuilder api)
    {
        api.MapGetPlaylists();
        api.MapGetPlaylistSummary();
        api.MapGetPlaylist();
        api.MapCreatePlaylist();
        api.MapRenamePlaylist();
        api.MapDeletePlaylist();
        api.MapAddPlaylistTrack();
        api.MapRemovePlaylistTrack();
        api.MapLoadPlaylist();
        api.MapGenerateAutomaticPlaylist();
        api.MapGetAutomaticPlaylistOptions();
    }
}
