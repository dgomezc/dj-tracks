using System.Text.Json;
using DjTracks.Infrastructure.Library;
using DjTracks.Infrastructure.Settings;

namespace DjTracks.Api.Features.Library;

public static class LibraryEndpoints
{
    public static void MapLibraryEndpoints(this WebApplication app)
    {
        app.MapGet("/api/library/tracks", async (string? year, string? style,
            JsonSettingsStore settingsStore, TrackLibrary library, ILoggerFactory loggerFactory,
            CancellationToken ct) =>
        {
            var logger = loggerFactory.CreateLogger("Library");
            try
            {
                var settings = await settingsStore.LoadAsync(ct);
                if (string.IsNullOrWhiteSpace(settings.Tracks))
                    return Results.Problem(statusCode: 409, title: "Configura la ruta de Tracks.",
                        detail: "Abre Configuración y guarda las rutas antes de explorar la biblioteca.");

                var listing = library.Browse(settings.Tracks, year, style, ct);
                foreach (var track in listing.Tracks.Where(track => track.Warning is not null))
                    logger.LogWarning("No se pudieron leer los tags de {Year}/{Style}/{FileName}",
                        year, style, track.FileName);
                return Results.Ok(listing);
            }
            catch (ArgumentException ex)
            {
                return Results.Problem(statusCode: 400, title: "Selección no válida.", detail: ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return Results.Problem(statusCode: 404, title: "Carpeta no encontrada.", detail: ex.Message);
            }
            catch (JsonException ex)
            {
                logger.LogError(ex, "JSON de ajustes inválido al explorar Tracks");
                return Results.Problem(statusCode: 500, title: "El JSON de ajustes es inválido.",
                    detail: "Corrige o restaura los ajustes del servidor y vuelve a cargar.");
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException
                or NotSupportedException)
            {
                logger.LogError(ex, "No se pudo explorar la biblioteca de Tracks");
                return Results.Problem(statusCode: 503, title: "No se pudo leer la biblioteca.",
                    detail: "Comprueba la ruta de Tracks, los permisos y la disponibilidad del almacenamiento y vuelve a cargar.");
            }
        }).WithTags("Library");
    }
}
