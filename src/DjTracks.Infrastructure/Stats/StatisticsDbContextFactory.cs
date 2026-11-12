using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DjTracks.Infrastructure.Stats;

public sealed class StatisticsDbContextFactory : IDesignTimeDbContextFactory<StatisticsDbContext>
{
    public StatisticsDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Statistics")
            ?? "Host=localhost;Database=djtracks;Username=postgres";
        var options = new DbContextOptionsBuilder<StatisticsDbContext>()
            .UseNpgsql(connectionString)
            .Options;
        return new StatisticsDbContext(options);
    }
}
