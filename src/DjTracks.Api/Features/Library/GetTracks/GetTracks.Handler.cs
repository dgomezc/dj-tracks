using DjTracks.Domain;
using DjTracks.Infrastructure.Library;
using DjTracks.Infrastructure.Settings;
using FluentResults;

namespace DjTracks.Api.Features.Library.GetTracks;

public sealed class GetTracksHandler(JsonSettingsStore settingsStore, TrackLibrary library,
    ILogger<GetTracksHandler> logger)
{
    public async Task<Result<TrackListing>> HandleAsync(GetTracksRequest request, CancellationToken ct)
    {
        var settings = await settingsStore.LoadAsync(ct);
        if (settings.IsFailed)
            return settings.ToResult();
        if (string.IsNullOrWhiteSpace(settings.Value.Tracks))
            return new AppError(ErrorKind.Conflict, "Configura la ruta de Tracks.",
                "Abre Configuración y guarda las rutas antes de explorar la biblioteca.");

        try
        {
            var listing = library.Browse(settings.Value.Tracks, request.Year, request.Style, ct);
            if (listing.IsSuccess)
                foreach (var track in listing.Value.Tracks.Where(track => track.Warning is not null))
                    logger.LogWarning("No se pudieron leer los tags de {Year}/{Style}/{FileName}",
                        request.Year, request.Style, track.FileName);
            return listing;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException)
        {
            logger.LogError(ex, "No se pudo explorar la biblioteca de Tracks");
            return new AppError(ErrorKind.Unavailable, "No se pudo leer la biblioteca.",
                "Comprueba la ruta de Tracks, los permisos y la disponibilidad del almacenamiento y vuelve a cargar.");
        }
    }
}
