using DjTracks.Api.Features.Subsonic.GetMusicDirectory;
using DjTracks.Domain;
using DjTracks.Infrastructure.Playlists;
using FluentResults;

namespace DjTracks.Api.Features.Subsonic.GetPlaylists;

public sealed class GetPlaylistsHandler(M3uPlaylistStore store, PlaylistTrackResolver resolver,
    IConfiguration configuration)
{
    public async Task<Result<SubsonicPlaylist[]>> HandleAsync(CancellationToken ct)
    {
        var snapshots = await store.ListSnapshotsAsync(ct);
        if (snapshots.IsFailed)
            return snapshots.ToResult();
        List<SubsonicPlaylist> playlists = [];
        foreach (var snapshot in snapshots.Value)
        {
            var result = Convert(snapshot, ct);
            if (result.IsFailed)
                return result.ToResult();
            playlists.Add(result.Value with { Entry = null });
        }
        return playlists.ToArray();
    }

    public async Task<Result<SubsonicPlaylist>> GetAsync(Guid id, CancellationToken ct)
    {
        var snapshot = await store.GetSnapshotAsync(id, ct);
        return snapshot.IsFailed ? snapshot.ToResult() : Convert(snapshot.Value, ct);
    }

    private Result<SubsonicPlaylist> Convert(PlaylistSnapshot snapshot, CancellationToken ct)
    {
        var result = resolver.Resolve(snapshot.Playlist.Entries.Select(entry => entry.Track), ct);
        if (result.IsFailed)
            return result.ToResult();
        if (result.Value.Any(item => item.Details.DurationSeconds is not { } seconds
            || !double.IsFinite(seconds) || seconds < 0))
            return new AppError(ErrorKind.Unavailable, "Duración no disponible.",
                "No se pudo leer la duración de una canción de la playlist.");

        var entries = result.Value.Select(item =>
        {
            var reference = item.Reference;
            var parent = reference.Library == "Tracks"
                ? new SubsonicId("Tracks", [reference.Year!, reference.Style!])
                : new SubsonicId("Remember", [reference.Category!]);
            return SubsonicChild.Song(parent.Child(reference.FileName), parent, item.Details);
        }).ToArray();
        return new SubsonicPlaylist(snapshot.Playlist.Id.ToString("N"), snapshot.Playlist.Name,
            configuration["Subsonic:Username"]!, false, true, entries.Length,
            entries.Sum(entry => (long)entry.Duration!.Value), snapshot.CreatedUtc, snapshot.ChangedUtc, entries);
    }
}

public sealed record SubsonicPlaylist(string Id, string Name, string Owner, bool Public, bool Readonly,
    int SongCount, long Duration, DateTime Created, DateTime Changed, SubsonicChild[]? Entry);
