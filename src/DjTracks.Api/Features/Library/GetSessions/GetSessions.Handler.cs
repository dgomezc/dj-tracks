using DjTracks.Domain;
using DjTracks.Infrastructure.Library;
using DjTracks.Infrastructure.Settings;
using FluentResults;

namespace DjTracks.Api.Features.Library.GetSessions;

public sealed class GetSessionsHandler(PathSettings settings, SessionLibrary library,
    ILogger<GetSessionsHandler> logger)
{
    public Result<SessionListing> Handle(GetSessionsRequest request, CancellationToken ct)
    {
        try
        {
            var result = library.Browse(settings.Sessions, request.Year, request.Session, ct);
            if (result.IsSuccess)
                foreach (var file in result.Value.Files.Where(file => file.Warning is not null))
                    logger.LogWarning("No se pudieron leer los tags de {Year}/{Session}/{FileName}",
                        request.Year, request.Session, file.FileName);
            return result;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException)
        {
            logger.LogError(ex, "No se pudo explorar la biblioteca de Sessions");
            return new AppError(ErrorKind.Unavailable, "No se pudieron leer las sesiones.",
                "Comprueba la ruta de Sessions, los permisos y la disponibilidad del almacenamiento y vuelve a cargar.");
        }
    }
}
