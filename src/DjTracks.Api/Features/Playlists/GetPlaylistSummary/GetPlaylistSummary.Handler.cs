using DjTracks.Domain;
using DjTracks.Infrastructure.Playlists;
using FluentResults;

namespace DjTracks.Api.Features.Playlists.GetPlaylistSummary;

public sealed class GetPlaylistSummaryHandler(M3uPlaylistStore store)
{
    public Task<Result<PlaylistSummary>> HandleAsync(CancellationToken ct) => store.SummarizeAsync(ct);
}
