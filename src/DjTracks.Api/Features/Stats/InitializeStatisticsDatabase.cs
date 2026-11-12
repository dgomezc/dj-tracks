using System.Data.Common;
using DjTracks.Infrastructure.Stats;
using Microsoft.EntityFrameworkCore;

namespace DjTracks.Api.Features.Stats;

public sealed class InitializeStatisticsDatabase(IServiceScopeFactory scopeFactory,
    IConfiguration configuration, ILogger<InitializeStatisticsDatabase> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (string.IsNullOrWhiteSpace(configuration.GetConnectionString("Statistics")))
        {
            logger.LogWarning("Estadísticas desactivadas: configura ConnectionStrings:Statistics en user-secrets o entorno.");
            return;
        }

        await using var scope = scopeFactory.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<StatisticsDbContext>();
        try
        {
            await database.Database.MigrateAsync(stoppingToken);
            logger.LogInformation("Base de datos de estadísticas migrada correctamente");
        }
        catch (DbException ex)
        {
            logger.LogError(ex, "No se pudo migrar la base de datos de estadísticas; la reproducción seguirá disponible");
        }
    }
}
