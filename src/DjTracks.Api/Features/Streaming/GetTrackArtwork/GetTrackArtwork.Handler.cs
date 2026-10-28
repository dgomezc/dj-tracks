using DjTracks.Domain;
using DjTracks.Infrastructure.Library;
using DjTracks.Infrastructure.Settings;
using FluentResults;

namespace DjTracks.Api.Features.Streaming.GetTrackArtwork;

public sealed class GetTrackArtworkHandler(JsonSettingsStore settingsStore, TrackLibrary library,
    ILogger<GetTrackArtworkHandler> logger)
{
    public async Task<Result<TrackArtwork>> HandleAsync(GetTrackArtworkRequest request, CancellationToken ct)
    {
        var settings = await settingsStore.LoadAsync(ct);
        if (settings.IsFailed)
            return settings.ToResult();
        if (string.IsNullOrWhiteSpace(settings.Value.Tracks))
            return new AppError(ErrorKind.Conflict, "Configura la ruta de Tracks.",
                "Abre Configuración y guarda las rutas antes de cargar la carátula.");

        try
        {
            var result = library.ReadArtwork(settings.Value.Tracks, request.Year!,
                request.Style!, request.FileName!, ct);
            if (result.IsFailed)
                logger.LogWarning("No se pudo cargar la carátula de {FileName}: {Error}",
                    request.FileName, result.Errors[0].Message);
            return result;
        }
        catch (FileNotFoundException ex)
        {
            logger.LogInformation(ex, "El fichero de audio dejó de estar disponible");
            return new AppError(ErrorKind.NotFound, "Canción no encontrada.",
                "El fichero seleccionado ya no existe. Vuelve a cargar la biblioteca.");
        }
        catch (Exception ex) when (ex is TagLib.CorruptFileException or TagLib.UnsupportedFormatException)
        {
            logger.LogWarning(ex, "No se pudieron leer los tags de {FileName}", request.FileName);
            return new AppError(ErrorKind.Unavailable, "No se pudo leer la carátula.",
                "No se pudieron leer los tags del MP3. Comprueba el fichero.");
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException)
        {
            logger.LogError(ex, "No se pudo abrir el fichero para leer su carátula");
            return new AppError(ErrorKind.Unavailable, "No se pudo cargar la carátula.",
                "Comprueba la ruta de Tracks, los permisos y la disponibilidad del almacenamiento.");
        }
    }
}
