using DjTracks.Domain;
using DjTracks.Infrastructure.Playlists;
using FluentResults;

namespace DjTracks.Api.Features.Playlists.RenamePlaylist;

public sealed class RenamePlaylistHandler(JsonPlaylistStore store)
{
    public Task<Result<ManualPlaylist>> HandleAsync(Guid id, RenamePlaylistRequest request, CancellationToken ct) =>
        store.RenameAsync(id, request.Name!.Trim(), ct);
}
