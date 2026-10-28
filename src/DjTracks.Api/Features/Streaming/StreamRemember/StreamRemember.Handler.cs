using DjTracks.Domain;
using DjTracks.Infrastructure.Library;
using DjTracks.Infrastructure.Settings;
using FluentResults;

namespace DjTracks.Api.Features.Streaming.StreamRemember;

public sealed class StreamRememberHandler(JsonSettingsStore settingsStore, RememberLibrary library,
    ILogger<StreamRememberHandler> logger)
{
    public async Task<Result<FileStream>> HandleAsync(StreamRememberRequest request, CancellationToken ct)
    {
        var settings = await settingsStore.LoadAsync(ct);
        if (settings.IsFailed)
            return settings.ToResult();
        if (string.IsNullOrWhiteSpace(settings.Value.Remember))
            return new AppError(ErrorKind.Conflict, "Configura la ruta de Remember.",
                "Abre Configuración y guarda las rutas antes de reproducir Remember.");

        try
        {
            return library.OpenAudio(settings.Value.Remember, request.Category!, request.FileName!, ct);
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
