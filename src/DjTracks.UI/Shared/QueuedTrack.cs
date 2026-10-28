namespace DjTracks.UI.Shared;

public sealed record QueuedTrack(Guid Id, string AudioUrl, string WaveformUrl,
    string Title, string? Artist);

public enum QueueRepeatMode
{
    Off,
    All,
    One
}
