using DjTracks.Domain;
using DjTracks.Infrastructure.Playlists;
using FluentResults;

namespace DjTracks.Api.Features.Playlists.CreatePlaylist;

public sealed class CreatePlaylistHandler(JsonPlaylistStore store)
{
    public Task<Result<ManualPlaylist>> HandleAsync(CreatePlaylistRequest request, CancellationToken ct) =>
        store.CreateAsync(request.Name!.Trim(), ct);
}
