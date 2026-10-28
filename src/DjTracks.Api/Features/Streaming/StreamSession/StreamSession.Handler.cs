using DjTracks.Domain;
using DjTracks.Infrastructure.Library;
using DjTracks.Infrastructure.Settings;
using FluentResults;

namespace DjTracks.Api.Features.Streaming.StreamSession;

public sealed class StreamSessionHandler(JsonSettingsStore settingsStore, SessionLibrary library,
    ILogger<StreamSessionHandler> logger)
{
    public async Task<Result<FileStream>> HandleAsync(StreamSessionRequest request, CancellationToken ct)
    {
        var settings = await settingsStore.LoadAsync(ct);
        if (settings.IsFailed)
            return settings.ToResult();
        if (string.IsNullOrWhiteSpace(settings.Value.Sessions))
            return new AppError(ErrorKind.Conflict, "Configura la ruta de Sessions.",
                "Abre Configuración y guarda las rutas antes de reproducir sesiones.");

        try
        {
            return library.OpenAudio(settings.Value.Sessions, request.Year!, request.Session!, request.FileName!, ct);
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
