using DjTracks.Domain;
using DjTracks.Infrastructure.Library;
using DjTracks.Infrastructure.Settings;
using FluentResults;

namespace DjTracks.Api.Features.Streaming.StreamSession;

public sealed class StreamSessionHandler(PathSettings settings, SessionLibrary library,
    ILogger<StreamSessionHandler> logger)
{
    public Result<FileStream> Handle(StreamSessionRequest request, CancellationToken ct)
    {
        try
        {
            return library.OpenAudio(settings.Sessions, request.Year!, request.Session!, request.FileName!, ct);
        }
        catch (FileNotFoundException ex)
        {
            logger.LogInformation(ex, "El fichero de la sesión dejó de estar disponible");
            return new AppError(ErrorKind.NotFound, "Sesión no encontrada.",
                "El fichero seleccionado ya no existe. Vuelve a cargar las sesiones.");
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException)
        {
            logger.LogError(ex, "No se pudo abrir el audio de la sesión");
            return new AppError(ErrorKind.Unavailable, "No se pudo reproducir la sesión.",
                "Comprueba la ruta de Sessions, los permisos y la disponibilidad del almacenamiento.");
        }
    }
}
