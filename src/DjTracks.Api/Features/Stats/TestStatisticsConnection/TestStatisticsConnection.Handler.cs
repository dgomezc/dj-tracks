using System.Data.Common;
using DjTracks.Domain;
using DjTracks.Infrastructure.Stats;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace DjTracks.Api.Features.Stats.TestStatisticsConnection;

public sealed class TestStatisticsConnectionHandler(StatisticsDbContext database,
    IConfiguration configuration, ILogger<TestStatisticsConnectionHandler> logger)
{
    public async Task<Result<StatisticsConnectionStatus>> HandleAsync(CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(configuration.GetConnectionString("Statistics")))
        {
            logger.LogWarning("Prueba de PostgreSQL solicitada sin conexión de estadísticas configurada");
            return new AppError(ErrorKind.Unavailable, "Conexión no configurada.",
                "Configura ConnectionStrings:Statistics en user-secrets de la API y reiníciala.");
        }

        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeout.CancelAfter(TimeSpan.FromSeconds(10));
        var opened = false;
        try
        {
            await database.Database.OpenConnectionAsync(timeout.Token);
            opened = true;
            var applied = (await database.Database.GetAppliedMigrationsAsync(timeout.Token)).ToArray();
            var pending = database.Database.GetMigrations().Except(applied).ToArray();
            logger.LogInformation("Prueba de PostgreSQL correcta: {Applied} migraciones aplicadas y {Pending} pendientes",
                applied.Length, pending.Length);
            return new StatisticsConnectionStatus(applied, pending);
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            logger.LogWarning("Tiempo de espera agotado al comprobar PostgreSQL");
            return new AppError(ErrorKind.Unavailable, "Tiempo de espera agotado.",
                "PostgreSQL no respondió en 10 segundos. Comprueba el servidor y la red.");
        }
        catch (ArgumentException)
        {
            logger.LogWarning("La configuración de conexión de estadísticas no tiene un formato válido");
            return new AppError(ErrorKind.Unavailable, "Configuración no válida.",
                "Revisa el formato de ConnectionStrings:Statistics en la configuración de la API.");
        }
        catch (Exception ex) when (ex is DbException or TimeoutException
            or InvalidOperationException { InnerException: DbException or TimeoutException })
        {
            logger.LogWarning("No se pudo comprobar PostgreSQL ({ExceptionType})", ex.GetType().Name);
            return new AppError(ErrorKind.Unavailable, "No se pudo comprobar PostgreSQL.",
                "Comprueba el servidor, la red, las credenciales y los permisos para leer el historial de migraciones.");
        }
        finally
        {
            if (opened)
                await database.Database.CloseConnectionAsync();
        }
    }
}
