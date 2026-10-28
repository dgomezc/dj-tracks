using DjTracks.Domain;
using DjTracks.Infrastructure.Playlists;
using FluentResults;

namespace DjTracks.Api.Features.Playlists.RemovePlaylistTrack;

public sealed class RemovePlaylistTrackHandler(JsonPlaylistStore store)
{
    public Task<Result<ManualPlaylist>> HandleAsync(RemovePlaylistTrackRequest request, CancellationToken ct) =>
        store.RemoveAsync(request.Id, request.EntryId, ct);
}
