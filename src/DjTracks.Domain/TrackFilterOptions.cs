namespace DjTracks.Domain;

public sealed record TrackFilterOptions(
    string[] Years,
    string[] Groups);
