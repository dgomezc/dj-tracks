namespace DjTracks.Domain;

public sealed record TrackListing(string? Year, string? Style, string[] Folders, TrackFile[] Tracks);

public sealed record TrackFile(string FileName, string? Title, string[] Artists,
    string? Album, double? DurationSeconds, string? Warning, string? Year = null, string? Style = null);

public sealed record TrackFilterOptions(string[] Years, string[] Groups);

public sealed record TrackFilter(string? Year = null, string? Group = null, string? Substyle = null);
