using DjTracks.Domain;

namespace DjTracks.UI.Shared;

public sealed class PlaylistChanges
{
    public event Action? Changed;
    public event Action<ManualPlaylist>? Renamed;

    public void Notify() => Changed?.Invoke();

    public void NotifyRenamed(ManualPlaylist playlist) => Renamed?.Invoke(playlist);
}