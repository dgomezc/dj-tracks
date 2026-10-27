using DjTracks.Domain;
using DjTracks.Infrastructure.Library;
using DjTracks.Infrastructure.Settings;
using DjTracks.Infrastructure.Streaming;
using FluentResults;

namespace DjTracks.Api.Features.Streaming.GetTrackWaveform;

public sealed class GetTrackWaveformHandler(JsonSettingsStore settingsStore, TrackLibrary library,
    TrackWaveformCache waveformCache, ILogger<GetTrackWaveformHandler> logger)
{
    public async Task<Result<TrackWaveform>> HandleAsync(GetTrackWaveformRequest request,
        CancellationToken ct)
    {
        var settings = await settingsStore.LoadAsync(ct);
        if (settings.IsFailed)
            return settings.ToResult();
        if (string.IsNullOrWhiteSpace(settings.Value.Tracks))
            return new AppError(ErrorKind.Conflict, "Configura la ruta de Tracks.",
                "Abre Configuración y guarda las rutas antes de cargar la forma de onda.");

        try
        {
            var file = library.ResolveTrackFile(settings.Value.Tracks, request.Year!,
                request.Style!, request.FileName!, ct);
            return file.IsFailed
                ? file.ToResult()
                : await waveformCache.GetAsync(file.Value, ct);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (FileNotFoundException ex)
        {
            logger.LogInformation(ex, "El fichero de audio dejó de estar disponible");
            return new AppError(ErrorKind.NotFound, "Canción no encontrada.",
                "El fichero seleccionado ya no existe. Vuelve a cargar la biblioteca.");
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException)
        {
            logger.LogError(ex, "No se pudo resolver el fichero para generar su forma de onda");
            return new AppError(ErrorKind.Unavailable, "No se pudo generar la forma de onda.",
                "Comprueba la ruta de Tracks, los permisos y la disponibilidad de FFmpeg.");
        }
    }
}
