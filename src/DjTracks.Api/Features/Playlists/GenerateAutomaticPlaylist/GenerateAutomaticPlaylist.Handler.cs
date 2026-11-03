using DjTracks.Domain;
using DjTracks.Infrastructure.Playlists;
using FluentResults;

namespace DjTracks.Api.Features.Playlists.GenerateAutomaticPlaylist;

public sealed class GenerateAutomaticPlaylistHandler(M3uPlaylistStore store)
{
    public Task<Result<ManualPlaylist>> HandleAsync(Guid? id, GenerateAutomaticPlaylistRequest request,
        CancellationToken ct) =>
        store.GenerateAsync(id, request.Name!.Trim(), request.Filters!, ct);
}
