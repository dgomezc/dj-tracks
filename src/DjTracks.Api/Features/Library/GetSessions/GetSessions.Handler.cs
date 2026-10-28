using DjTracks.Domain;
using DjTracks.Infrastructure.Library;
using DjTracks.Infrastructure.Settings;
using FluentResults;

namespace DjTracks.Api.Features.Library.GetSessions;

public sealed class GetSessionsHandler(JsonSettingsStore settingsStore, SessionLibrary library,
    ILogger<GetSessionsHandler> logger)
{
    public async Task<Result<SessionListing>> HandleAsync(GetSessionsRequest request, CancellationToken ct)
    {
        var settings = await settingsStore.LoadAsync(ct);
        if (settings.IsFailed)
            return settings.ToResult();
        if (string.IsNullOrWhiteSpace(settings.Value.Sessions))
            return new AppError(ErrorKind.Conflict, "Configura la ruta de Sessions.",
                "Abre Configuración y guarda las rutas antes de explorar las sesiones.");

        try
        {
            var result = library.Browse(settings.Value.Sessions, request.Year, request.Session, ct);
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
