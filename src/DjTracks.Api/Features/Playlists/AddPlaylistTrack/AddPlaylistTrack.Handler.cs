using DjTracks.Domain;
using DjTracks.Infrastructure.Playlists;
using FluentResults;

namespace DjTracks.Api.Features.Playlists.AddPlaylistTrack;

public sealed class AddPlaylistTrackHandler(JsonPlaylistStore store, PlaylistTrackResolver resolver)
{
    public async Task<Result<ManualPlaylist>> HandleAsync(Guid id, AddPlaylistTrackRequest request, CancellationToken ct)
    {
        var playlist = await store.GetAsync(id, ct);
        if (playlist.IsFailed)
            return playlist;
        var resolved = await resolver.ResolveAsync([request.Track!], ct);
        return resolved.IsFailed ? resolved.ToResult() : await store.AddAsync(id, request.Track!, ct);
    }
}
