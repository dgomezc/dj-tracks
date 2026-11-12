namespace DjTracks.Domain;

public sealed record PlaybackStatistics(long TotalPlays, int UniqueFiles,
    DateTimeOffset? FromUtc, DateTimeOffset UntilUtc,
    RankedPlayback[] MostPlayed, PlaybackHistoryEntry[] RecentPlays);

public sealed record PlaybackHistoryEntry(Guid EventId, string Library, string RelativePath,
    string FileName, string? Title, string? Artist, DateTimeOffset PlayedAtUtc);

public sealed record RankedPlayback(PlaybackHistoryEntry LatestPlay, long PlayCount);

public sealed record LibrarySummary(string Library, int AudioFiles);

public sealed record PlaylistSummary(int ManualCount, int AutomaticCount);
