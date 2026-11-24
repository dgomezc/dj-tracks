using DjTracks.Domain;

namespace DjTracks.Infrastructure.Playlists;

public sealed record PlaylistSnapshot(ManualPlaylist Playlist, DateTime CreatedUtc, DateTime ChangedUtc);
