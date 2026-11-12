namespace DjTracks.Infrastructure.Stats;

public sealed class PlaybackEvent
{
    public Guid EventId { get; set; }
    public string Library { get; set; } = "";
    public string RelativePath { get; set; } = "";
    public string FileName { get; set; } = "";
    public string? Year { get; set; }
    public string? Style { get; set; }
    public string? Category { get; set; }
    public string? SessionFolder { get; set; }
    public string? Title { get; set; }
    public string? Artist { get; set; }
    public DateTimeOffset PlayedAtUtc { get; set; }
}
