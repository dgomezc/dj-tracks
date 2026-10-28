using DjTracks.Domain;
using DjTracks.Infrastructure.Playlists;
using FluentResults;

namespace DjTracks.Api.Features.Playlists.LoadPlaylist;

public sealed class LoadPlaylistHandler(JsonPlaylistStore store, PlaylistTrackResolver resolver)
{
    public async Task<Result<PlaylistPlayback>> HandleAsync(LoadPlaylistRequest request, CancellationToken ct)
    {
        var playlist = await store.GetAsync(request.Id, ct);
        if (playlist.IsFailed)
            return playlist.ToResult();
        var tracks = await resolver.ResolveAsync(playlist.Value.Entries.Select(entry => entry.Track), ct);
        return tracks.IsFailed ? tracks.ToResult() : new PlaylistPlayback(tracks.Value);
    }
}
