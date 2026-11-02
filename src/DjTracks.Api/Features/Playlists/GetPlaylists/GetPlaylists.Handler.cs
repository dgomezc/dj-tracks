using DjTracks.Domain;
using DjTracks.Infrastructure.Playlists;
using FluentResults;

namespace DjTracks.Api.Features.Playlists.GetPlaylists;

public sealed class GetPlaylistsHandler(JsonPlaylistStore store)
{
    public Task<Result<ManualPlaylist[]>> HandleAsync(CancellationToken ct) => store.ListAsync(ct);
}
