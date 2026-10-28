using DjTracks.Domain;
using DjTracks.Infrastructure.Library;
using DjTracks.Infrastructure.Settings;
using DjTracks.Infrastructure.Streaming;
using FluentResults;

namespace DjTracks.Api.Features.Streaming.GetRememberWaveform;

public sealed class GetRememberWaveformHandler(JsonSettingsStore settingsStore, RememberLibrary library,
    TrackWaveformCache waveformCache, ILogger<GetRememberWaveformHandler> logger)
{
    public async Task<Result<TrackWaveform>> HandleAsync(GetRememberWaveformRequest request,
        Func<TrackWaveformChunk, CancellationToken, ValueTask> onChunk, CancellationToken ct)
    {
        var settings = await settingsStore.LoadAsync(ct);
        if (settings.IsFailed)
            return settings.ToResult();
        if (string.IsNullOrWhiteSpace(settings.Value.Remember))
            return new AppError(ErrorKind.Conflict, "Configura la ruta de Remember.",
                "Abre Configuración y guarda las rutas antes de cargar la forma de onda.");

        try
        {
            var file = library.ResolveAudioFile(settings.Value.Remember, request.Category!, request.FileName!, ct);
            return file.IsFailed ? file.ToResult() : await waveformCache.GetAsync(file.Value, ct, onChunk);
        }
        catch (FileNotFoundException ex)
        {
            logger.LogInformation(ex, "El fichero de Remember dejó de estar disponible");
            return new AppError(ErrorKind.NotFound, "Canción no encontrada.",
                "El fichero seleccionado ya no existe. Vuelve a cargar Remember.");
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException)
        {
            logger.LogError(ex, "No se pudo resolver el fichero de Remember para generar su forma de onda");
            return new AppError(ErrorKind.Unavailable, "No se pudo generar la forma de onda.",
                "Comprueba la ruta de Remember, los permisos y la disponibilidad de FFmpeg.");
        }
    }
}
