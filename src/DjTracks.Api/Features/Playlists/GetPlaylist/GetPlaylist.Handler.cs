using DjTracks.Domain;
using DjTracks.Infrastructure.Playlists;
using FluentResults;

namespace DjTracks.Api.Features.Playlists.GetPlaylist;

public sealed class GetPlaylistHandler(JsonPlaylistStore store)
{
    public Task<Result<ManualPlaylist>> HandleAsync(GetPlaylistRequest request, CancellationToken ct) =>
        store.GetAsync(request.Id, ct);
}
