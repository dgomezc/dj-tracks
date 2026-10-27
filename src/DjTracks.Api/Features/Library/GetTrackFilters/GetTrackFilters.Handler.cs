using DjTracks.Domain;
using DjTracks.Infrastructure.Library;
using DjTracks.Infrastructure.Settings;
using FluentResults;

namespace DjTracks.Api.Features.Library.GetTrackFilters;

public sealed class GetTrackFiltersHandler(JsonSettingsStore settingsStore, TrackLibrary library,
    ILogger<GetTrackFiltersHandler> logger)
{
    public async Task<Result<TrackFilterOptions>> HandleAsync(CancellationToken ct)
    {
        var settings = await settingsStore.LoadAsync(ct);
        if (settings.IsFailed)
            return settings.ToResult();
        if (string.IsNullOrWhiteSpace(settings.Value.Tracks))
            return new AppError(ErrorKind.Conflict, "Configura la ruta de Tracks.",
                "Abre Configuración y guarda las rutas antes de explorar la biblioteca.");

        try
        {
            return library.GetFilterOptions(settings.Value.Tracks, ct);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException)
        {
            logger.LogError(ex, "No se pudieron cargar los filtros de la biblioteca de Tracks");
            return new AppError(ErrorKind.Unavailable, "No se pudo leer la biblioteca.",
                "Comprueba la ruta de Tracks, los permisos y la disponibilidad del almacenamiento y vuelve a cargar.");
        }
    }
}
