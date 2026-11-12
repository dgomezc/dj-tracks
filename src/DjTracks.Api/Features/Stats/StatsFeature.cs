using DjTracks.Api.Features.Stats.RecordPlayback;
using DjTracks.Infrastructure.Stats;
using Microsoft.EntityFrameworkCore;

namespace DjTracks.Api.Features.Stats;

public static class StatsFeature
{
    public static IServiceCollection AddStatsFeature(this IServiceCollection services)
    {
        services.AddDbContext<StatisticsDbContext>((provider, options) =>
        {
            var connectionString = provider.GetRequiredService<IConfiguration>()
                .GetConnectionString("Statistics");
            if (!string.IsNullOrWhiteSpace(connectionString))
                options.UseNpgsql(connectionString);
        });
        return services.AddScoped<RecordPlaybackHandler>()
            .AddHostedService<InitializeStatisticsDatabase>();
    }

    public static void MapStatsFeature(this RouteGroupBuilder api) =>
        api.MapRecordPlayback();
}
