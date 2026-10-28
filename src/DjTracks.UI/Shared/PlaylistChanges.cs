namespace DjTracks.UI.Shared;

public sealed class PlaylistChanges
{
    public event Action? Changed;

    public void Notify() => Changed?.Invoke();
}
