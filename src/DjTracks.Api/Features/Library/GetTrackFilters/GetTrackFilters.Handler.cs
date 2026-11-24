using DjTracks.Domain;
using DjTracks.Infrastructure.Library;
using DjTracks.Infrastructure.Settings;
using FluentResults;

namespace DjTracks.Api.Features.Library.GetTrackFilters;

public sealed class GetTrackFiltersHandler(PathSettings settings, TrackLibrary library,
    ILogger<GetTrackFiltersHandler> logger)
{
    public Result<TrackFilterOptions> Handle(CancellationToken ct)
    {
        try
        {
            return library.GetFilterOptions(settings.Tracks, ct);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException)
        {
            logger.LogError(ex, "No se pudieron cargar los filtros de la biblioteca de Tracks");
            return new AppError(ErrorKind.Unavailable, "No se pudo leer la biblioteca.",
                "Comprueba la ruta de Tracks, los permisos y la disponibilidad del almacenamiento y vuelve a cargar.");
        }
    }
}
