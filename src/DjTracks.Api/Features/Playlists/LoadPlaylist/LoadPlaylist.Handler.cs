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
        var entries = playlist.Value.Entries
            .Where(entry => request.EntryId is null || entry.Id == request.EntryId).ToArray();
        if (request.EntryId is not null && entries.Length == 0)
            return new AppError(ErrorKind.NotFound, "Canción no encontrada.",
                "La entrada ya no está en la playlist. Vuelve a cargarla.");
        var tracks = await resolver.ResolveAsync(entries.Select(entry => entry.Track), ct);
        return tracks.IsFailed ? tracks.ToResult() : new PlaylistPlayback(tracks.Value);
    }
}