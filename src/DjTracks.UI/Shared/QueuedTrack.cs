using DjTracks.Domain;

namespace DjTracks.UI.Shared;

public sealed record QueuedTrack(Guid Id, string AudioUrl, string? WaveformUrl,
    string? ArtworkUrl, TrackFile Details, string? SessionFolder = null, string? TracklistUrl = null,
    string? RememberCategory = null)
{
    public string Title => string.IsNullOrWhiteSpace(Details.Title) ? Details.FileName : Details.Title;
    public string Artist => string.Join(", ", Details.Artists);
}

public enum QueueRepeatMode
{
    Off,
    All,
    One
}
