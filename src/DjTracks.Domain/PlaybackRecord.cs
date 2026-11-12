namespace DjTracks.Domain;

public sealed record PlaybackRecord(Guid EventId, string Library, string FileName,
    string? Year = null, string? Style = null, string? Category = null, string? SessionFolder = null,
    string? Title = null, string[]? Artists = null);
