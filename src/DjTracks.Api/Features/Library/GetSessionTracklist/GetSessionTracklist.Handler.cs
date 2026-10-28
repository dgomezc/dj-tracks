using System.Text;
using DjTracks.Domain;
using DjTracks.Infrastructure.Library;
using DjTracks.Infrastructure.Settings;
using FluentResults;

namespace DjTracks.Api.Features.Library.GetSessionTracklist;

public sealed class GetSessionTracklistHandler(JsonSettingsStore settingsStore, SessionLibrary library,
    ILogger<GetSessionTracklistHandler> logger)
{
    public async Task<Result<SessionTracklist>> HandleAsync(GetSessionTracklistRequest request, CancellationToken ct)
    {
        var settings = await settingsStore.LoadAsync(ct);
        if (settings.IsFailed)
            return settings.ToResult();
        if (string.IsNullOrWhiteSpace(settings.Value.Sessions))
            return new AppError(ErrorKind.Conflict, "Configura la ruta de Sessions.",
                "Abre Configuración y guarda las rutas antes de cargar el tracklist.");

        try
        {
            return await library.ReadTracklistAsync(settings.Value.Sessions, request.Year!,
                request.Session!, request.FileName!, ct);
        }
        catch (FileNotFoundException ex)
        {
            logger.LogWarning(ex, "El fichero de la sesión o su tracklist dejó de estar disponible");
            return new AppError(ErrorKind.NotFound, "Fichero no encontrado.",
                "El fichero de la sesión o su tracklist ya no existe. Vuelve a cargar.");
        }
        catch (DecoderFallbackException ex)
        {
            logger.LogWarning(ex, "Codificación no válida del tracklist de {Year}/{Session}",
                request.Year, request.Session);
            return new AppError(ErrorKind.Validation, "No se pudo leer el tracklist.",
                "El TXT no tiene una codificación de texto válida. Guárdalo como UTF-8 y vuelve a cargar.");
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException)
        {
            logger.LogError(ex, "No se pudo leer el tracklist de {Year}/{Session}",
                request.Year, request.Session);
            return new AppError(ErrorKind.Unavailable, "No se pudo cargar el tracklist.",
                "Comprueba la ruta de Sessions, los permisos y la disponibilidad del almacenamiento y vuelve a cargar.");
        }
    }
}
