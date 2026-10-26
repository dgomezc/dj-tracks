namespace DjTracks.Domain;

public sealed record TrackListing(string? Year, string? Style, string[] Folders, TrackFile[] Tracks);

public sealed record TrackFile(string FileName, string? Title, string[] Artists,
    string? Album, double? DurationSeconds, string? Warning);
