using DjTracks.Domain;
using DjTracks.Infrastructure.Library;
using DjTracks.Infrastructure.Settings;
using DjTracks.Infrastructure.Streaming;
using FluentResults;

namespace DjTracks.Api.Features.Streaming.GetSessionWaveform;

public sealed class GetSessionWaveformHandler(PathSettings settings, SessionLibrary library,
    TrackWaveformCache waveformCache, ILogger<GetSessionWaveformHandler> logger)
{
    public async Task<Result<TrackWaveform>> HandleAsync(GetSessionWaveformRequest request,
        Func<TrackWaveformChunk, CancellationToken, ValueTask> onChunk, CancellationToken ct)
    {
        try
        {
            var file = library.ResolveAudioFile(settings.Sessions, request.Year!,
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
