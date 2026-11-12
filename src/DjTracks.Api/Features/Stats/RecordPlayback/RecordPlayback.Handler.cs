using System.Data.Common;
using DjTracks.Domain;
using DjTracks.Infrastructure.Stats;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace DjTracks.Api.Features.Stats.RecordPlayback;

public sealed class RecordPlaybackHandler(StatisticsDbContext database,
    IConfiguration configuration, ILogger<RecordPlaybackHandler> logger)
{
    public async Task<Result<Guid>> HandleAsync(RecordPlaybackRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(configuration.GetConnectionString("Statistics")))
            return DatabaseUnavailable("La conexión a PostgreSQL no está configurada. Añade ConnectionStrings:Statistics a user-secrets.");

        var playback = request.Playback!;
        var relativePath = playback.Library switch
        {
            "Tracks" => $"{playback.Year}/{playback.Style}/{playback.FileName}",
            "Sessions" => $"{playback.Year}/{playback.SessionFolder}/{playback.FileName}",
            _ => $"{playback.Category}/{playback.FileName}"
        };
        var entity = new PlaybackEvent
        {
            EventId = playback.EventId,
            Library = playback.Library,
            RelativePath = relativePath,
            FileName = playback.FileName,
            Year = playback.Year,
            Style = playback.Style,
            Category = playback.Category,
            SessionFolder = playback.SessionFolder,
            Title = playback.Title,
            Artist = playback.Artists is { Length: > 0 } ? string.Join(", ", playback.Artists) : null,
            PlayedAtUtc = DateTimeOffset.UtcNow
        };

        try
        {
            database.PlaybackEvents.Add(entity);
            await database.SaveChangesAsync(ct);
            logger.LogInformation("Reproducción registrada para {Library}/{RelativePath}", entity.Library, entity.RelativePath);
            return entity.EventId;
        }
        catch (Exception ex) when (ex is DbException or DbUpdateException)
        {
            logger.LogError(ex, "No se pudo registrar la reproducción {EventId}", entity.EventId);
            return DatabaseUnavailable("No se pudo guardar la reproducción en PostgreSQL. Comprueba la conexión y vuelve a intentarlo.");
        }
    }

    private static AppError DatabaseUnavailable(string detail) =>
        new(ErrorKind.Unavailable, "Estadísticas no disponibles.", detail);
}
