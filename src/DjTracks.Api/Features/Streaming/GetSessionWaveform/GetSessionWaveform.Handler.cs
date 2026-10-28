using DjTracks.Domain;
using DjTracks.Infrastructure.Library;
using DjTracks.Infrastructure.Settings;
using DjTracks.Infrastructure.Streaming;
using FluentResults;

namespace DjTracks.Api.Features.Streaming.GetSessionWaveform;

public sealed class GetSessionWaveformHandler(JsonSettingsStore settingsStore, SessionLibrary library,
    TrackWaveformCache waveformCache, ILogger<GetSessionWaveformHandler> logger)
{
    public async Task<Result<TrackWaveform>> HandleAsync(GetSessionWaveformRequest request,
        Func<TrackWaveformChunk, CancellationToken, ValueTask> onChunk, CancellationToken ct)
    {
        var settings = await settingsStore.LoadAsync(ct);
        if (settings.IsFailed)
            return settings.ToResult();
        if (string.IsNullOrWhiteSpace(settings.Value.Sessions))
            return new AppError(ErrorKind.Conflict, "Configura la ruta de Sessions.",
                "Abre Configuración y guarda las rutas antes de cargar la forma de onda.");

        try
        {
            var file = library.ResolveAudioFile(settings.Value.Sessions, request.Year!,
                request.Session!, request.FileName!, ct);
            return file.IsFailed
                ? file.ToResult()
                : await waveformCache.GetAsync(file.Value, ct, onChunk);
        }
        catch (FileNotFoundException ex)
        {
            logger.LogInformation(ex, "El fichero de la sesión dejó de estar disponible");
            return new AppError(ErrorKind.NotFound, "Sesión no encontrada.",
                "El fichero seleccionado ya no existe. Vuelve a cargar la biblioteca.");
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException)
        {
            logger.LogError(ex, "No se pudo resolver la sesión para generar su forma de onda");
            return new AppError(ErrorKind.Unavailable, "No se pudo generar la forma de onda.",
                "Comprueba la ruta de Sessions, los permisos y la disponibilidad de FFmpeg.");
        }
    }
}
