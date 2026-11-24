using DjTracks.Domain;
using DjTracks.Infrastructure.Library;
using DjTracks.Infrastructure.Settings;
using FluentResults;

namespace DjTracks.Api.Features.Streaming.StreamRemember;

public sealed class StreamRememberHandler(PathSettings settings, RememberLibrary library,
    ILogger<StreamRememberHandler> logger)
{
    public Result<FileStream> Handle(StreamRememberRequest request, CancellationToken ct)
    {
        try
        {
            return library.OpenAudio(settings.Remember, request.Category!, request.FileName!, ct);
        }
        catch (FileNotFoundException ex)
        {
            logger.LogInformation(ex, "El fichero de Remember dejó de estar disponible");
            return new AppError(ErrorKind.NotFound, "Canción no encontrada.",
                "El fichero seleccionado ya no existe. Vuelve a cargar Remember.");
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException)
        {
            logger.LogError(ex, "No se pudo abrir el audio de Remember");
            return new AppError(ErrorKind.Unavailable, "No se pudo reproducir Remember.",
                "Comprueba la ruta de Remember, los permisos y la disponibilidad del almacenamiento.");
        }
    }
}
