using System.Data.Common;
using DjTracks.Domain;
using DjTracks.Infrastructure.Stats;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace DjTracks.Api.Features.Stats.GetPlaybackStatistics;

public sealed class GetPlaybackStatisticsHandler(StatisticsDbContext database,
    IConfiguration configuration, ILogger<GetPlaybackStatisticsHandler> logger)
{
    public async Task<Result<PlaybackStatistics>> HandleAsync(GetPlaybackStatisticsRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(configuration.GetConnectionString("Statistics")))
            return new AppError(ErrorKind.Unavailable, "Estadísticas no configuradas.",
                "Configura la conexión a PostgreSQL y compruébala desde Configuración.");

        var until = DateTimeOffset.UtcNow;
        DateTimeOffset? from = request.Days is { } days ? until.AddDays(-days) : null;
        var plays = database.PlaybackEvents.AsNoTracking().Where(play => play.PlayedAtUtc <= until);
        if (from is { } start)
            plays = plays.Where(play => play.PlayedAtUtc >= start);

        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeout.CancelAfter(TimeSpan.FromSeconds(10));
        try
        {
            var total = await plays.LongCountAsync(timeout.Token);
            var unique = await plays.Select(play => new { play.Library, play.RelativePath })
                .Distinct().CountAsync(timeout.Token);
            var most = await plays.GroupBy(play => new { play.Library, play.RelativePath })
                .Select(group => new
                {
                    group.Key.Library,
                    group.Key.RelativePath,
                    Count = group.LongCount(),
                    LastPlayedAtUtc = group.Max(play => play.PlayedAtUtc),
                    Latest = group.OrderByDescending(play => play.PlayedAtUtc)
                        .ThenBy(play => play.EventId).First()
                })
                .OrderByDescending(group => group.Count).ThenByDescending(group => group.LastPlayedAtUtc)
                .ThenBy(group => group.Library).ThenBy(group => group.RelativePath)
                .Take(10).ToArrayAsync(timeout.Token);
            var recent = await plays.OrderByDescending(play => play.PlayedAtUtc).ThenBy(play => play.EventId)
                .Take(10).ToArrayAsync(timeout.Token);

            return new PlaybackStatistics(total, unique, from, until,
                most.Select(item => new RankedPlayback(ToEntry(item.Latest), item.Count)).ToArray(),
                recent.Select(ToEntry).ToArray());
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            logger.LogWarning("Tiempo de espera agotado al consultar estadísticas");
            return new AppError(ErrorKind.Unavailable, "Estadísticas no disponibles.",
                "PostgreSQL no respondió en 10 segundos. Comprueba la conexión desde Configuración.");
        }
        catch (Exception ex) when (ex is DbException or TimeoutException
            or InvalidOperationException { InnerException: DbException or TimeoutException })
        {
            logger.LogWarning("No se pudieron consultar las estadísticas ({ExceptionType})", ex.GetType().Name);
            return new AppError(ErrorKind.Unavailable, "Estadísticas no disponibles.",
                "No se pudieron leer las reproducciones. Comprueba la conexión y las migraciones desde Configuración.");
        }
    }

    private static PlaybackHistoryEntry ToEntry(PlaybackEvent play) =>
        new(play.EventId, play.Library, play.RelativePath, play.FileName, play.Title, play.Artist, play.PlayedAtUtc);
}
