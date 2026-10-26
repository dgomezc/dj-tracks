using System.Text.Json;
using DjTracks.Domain;
using DjTracks.Infrastructure.Library;
using DjTracks.Infrastructure.Settings;

namespace DjTracks.Api.Features.Library;

public static class LibraryEndpoints
{
    public static void MapLibraryEndpoints(this WebApplication app)
    {
        var routes = app.MapGroup("/api/library/tracks").WithTags("Library");
        routes.MapGet("/", (string? year, string? style,
            JsonSettingsStore settingsStore, TrackLibrary library, ILoggerFactory loggerFactory,
            CancellationToken ct) => ExecuteAsync(settingsStore, loggerFactory,
                root => library.Browse(root, year, style, ct), ct));
        routes.MapGet("/filter-options", (JsonSettingsStore settingsStore, TrackLibrary library,
            ILoggerFactory loggerFactory, CancellationToken ct) =>
            ExecuteAsync(settingsStore, loggerFactory, root => library.GetFilterOptions(root, ct), ct));
        routes.MapGet("/search", (string? year, string? group, string? substyle,
            JsonSettingsStore settingsStore, TrackLibrary library, ILoggerFactory loggerFactory,
            CancellationToken ct) => ExecuteAsync(settingsStore, loggerFactory,
                root => library.Search(root, new TrackFilter(year, group, substyle), ct), ct));
    }

    private static async Task<IResult> ExecuteAsync(JsonSettingsStore settingsStore,
        ILoggerFactory loggerFactory, Func<string, object> read, CancellationToken ct)
    {
        var logger = loggerFactory.CreateLogger("Library");
        try
        {
            var settings = await settingsStore.LoadAsync(ct);
            if (string.IsNullOrWhiteSpace(settings.Tracks))
                return Results.Problem(statusCode: 409, title: "Configura la ruta de Tracks.",
                    detail: "Abre Configuración y guarda las rutas antes de explorar la biblioteca.");

            var result = read(settings.Tracks);
            if (result is TrackListing listing)
                foreach (var track in listing.Tracks.Where(track => track.Warning is not null))
                    logger.LogWarning("No se pudieron leer los tags de {Year}/{Style}/{FileName}",
                        track.Year, track.Style, track.FileName);
            return Results.Ok(result);
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
    }
}
