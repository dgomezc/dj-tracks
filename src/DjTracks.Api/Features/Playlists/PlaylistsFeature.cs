using DjTracks.Api.Features.Playlists.AddPlaylistTrack;
using DjTracks.Api.Features.Playlists.CreatePlaylist;
using DjTracks.Api.Features.Playlists.DeletePlaylist;
using DjTracks.Api.Features.Playlists.GetPlaylist;
using DjTracks.Api.Features.Playlists.GetPlaylists;
using DjTracks.Api.Features.Playlists.LoadPlaylist;
using DjTracks.Api.Features.Playlists.RemovePlaylistTrack;
using DjTracks.Api.Features.Playlists.RenamePlaylist;
using DjTracks.Infrastructure.Playlists;

namespace DjTracks.Api.Features.Playlists;

public static class PlaylistsFeature
{
    public static IServiceCollection AddPlaylistsFeature(this IServiceCollection services) =>
        services.AddSingleton<JsonPlaylistStore>()
            .AddScoped<PlaylistTrackResolver>()
            .AddScoped<GetPlaylistsHandler>().AddScoped<GetPlaylistHandler>()
            .AddScoped<CreatePlaylistHandler>().AddScoped<RenamePlaylistHandler>()
            .AddScoped<DeletePlaylistHandler>().AddScoped<AddPlaylistTrackHandler>()
            .AddScoped<RemovePlaylistTrackHandler>().AddScoped<LoadPlaylistHandler>();

    public static void MapPlaylistsFeature(this RouteGroupBuilder api)
    {
        api.MapGetPlaylists();
        api.MapGetPlaylist();
        api.MapCreatePlaylist();
        api.MapRenamePlaylist();
        api.MapDeletePlaylist();
        api.MapAddPlaylistTrack();
        api.MapRemovePlaylistTrack();
        api.MapLoadPlaylist();
    }
}
