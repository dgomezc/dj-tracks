namespace DjTracks.Domain;

public sealed record FolderListing(string? CurrentPath, string? ParentPath, string[] Directories);
