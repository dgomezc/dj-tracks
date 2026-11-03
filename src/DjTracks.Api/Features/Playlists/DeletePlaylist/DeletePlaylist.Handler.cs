using DjTracks.Infrastructure.Playlists;
using FluentResults;

namespace DjTracks.Api.Features.Playlists.DeletePlaylist;

public sealed class DeletePlaylistHandler(M3uPlaylistStore store)
{
    public Task<Result<Guid>> HandleAsync(DeletePlaylistRequest request, CancellationToken ct) =>
        store.DeleteAsync(request.Id, ct);
}
