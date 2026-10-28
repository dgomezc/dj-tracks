namespace DjTracks.Domain;

public sealed record SessionListing(string? Year, string? Session, string[] Folders, TrackFile[] Files);
