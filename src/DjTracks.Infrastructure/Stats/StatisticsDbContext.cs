using Microsoft.EntityFrameworkCore;

namespace DjTracks.Infrastructure.Stats;

public sealed class StatisticsDbContext(DbContextOptions<StatisticsDbContext> options) : DbContext(options)
{
    public DbSet<PlaybackEvent> PlaybackEvents => Set<PlaybackEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var playback = modelBuilder.Entity<PlaybackEvent>();
        playback.ToTable("playback_events");
        playback.HasKey(item => item.EventId);
        playback.Property(item => item.Library).HasMaxLength(16).IsRequired();
        playback.Property(item => item.RelativePath).HasMaxLength(1024).IsRequired();
        playback.Property(item => item.FileName).HasMaxLength(255).IsRequired();
        playback.Property(item => item.Year).HasMaxLength(64);
        playback.Property(item => item.Style).HasMaxLength(128);
        playback.Property(item => item.Category).HasMaxLength(32);
        playback.Property(item => item.SessionFolder).HasMaxLength(255);
        playback.Property(item => item.Title).HasMaxLength(512);
        playback.Property(item => item.Artist).HasMaxLength(1024);
        playback.Property(item => item.PlayedAtUtc).IsRequired();
        playback.HasIndex(item => item.PlayedAtUtc);
        playback.HasIndex(item => new { item.Library, item.RelativePath });
    }
}
