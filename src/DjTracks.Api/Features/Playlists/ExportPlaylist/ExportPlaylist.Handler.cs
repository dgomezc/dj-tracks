using DjTracks.Domain;
using DjTracks.Infrastructure.Playlists;
using FluentResults;

namespace DjTracks.Api.Features.Playlists.ExportPlaylist;

public sealed class ExportPlaylistHandler(PlaylistM3uExporter exporter)
{
    public Task<Result<PlaylistM3uExport>> HandleAsync(ExportPlaylistRequest request, CancellationToken ct) =>
        exporter.ExportAsync(request.Id, ct);
}
