namespace DjTracks.Domain;

public sealed record StatisticsConnectionStatus(string[] AppliedMigrations, string[] PendingMigrations);
