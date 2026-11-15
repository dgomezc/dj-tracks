using DjTracks.Domain;
using DjTracks.Infrastructure.Library;
using DjTracks.Infrastructure.Settings;
using FluentResults;

namespace DjTracks.Api.Features.Streaming.StreamTrack;

public sealed class StreamTrackHandler(PathSettings settings, TrackLibrary library,
    ILogger<StreamTrackHandler> logger)
{
    public Result<FileStream> Handle(StreamTrackRequest request, CancellationToken ct)
    {
        try
        {
            return library.OpenTrack(settings.Tracks, request.Year!, request.Style!, request.FileName!, ct);
        }
        catch (FileNotFoundException ex)
        {
            logger.LogInformation(ex, "El fichero de audio dejó de estar disponible");
            return new AppError(ErrorKind.NotFound, "Canción no encontrada.",
                "El fichero seleccionado ya no existe. Vuelve a cargar la biblioteca.");
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException)
        {
            logger.LogError(ex, "No se pudo abrir el fichero de audio");
            return new AppError(ErrorKind.Unavailable, "No se pudo reproducir la canción.",
                "Comprueba la ruta de Tracks, los permisos y la disponibilidad del almacenamiento y vuelve a intentarlo.");
        }
    }
}
