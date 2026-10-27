using DjTracks.Domain;
using DjTracks.Infrastructure.Library;
using DjTracks.Infrastructure.Settings;
using FluentResults;

namespace DjTracks.Api.Features.Library.GetFilteredTracks;

public sealed class GetFilteredTracksHandler(JsonSettingsStore settingsStore, TrackLibrary library,
    ILogger<GetFilteredTracksHandler> logger)
{
    public async Task<Result<TrackListing>> HandleAsync(GetFilteredTracksRequest request, CancellationToken ct)
    {
        var settings = await settingsStore.LoadAsync(ct);
        if (settings.IsFailed)
            return settings.ToResult();
        if (string.IsNullOrWhiteSpace(settings.Value.Tracks))
            return new AppError(ErrorKind.Conflict, "Configura la ruta de Tracks.",
                "Abre Configuración y guarda las rutas antes de explorar la biblioteca.");

        try
        {
            var listing = library.Search(settings.Value.Tracks,
                new TrackFilter(request.Year ?? [], request.Group ?? [], request.Substyle ?? []), ct);
            if (listing.IsFailed)
                return listing;
            foreach (var track in listing.Value.Tracks.Where(track => track.Warning is not null))
                logger.LogWarning("No se pudieron leer los tags de {Year}/{Style}/{FileName}",
                    track.Year, track.Style, track.FileName);
            return listing;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException)
        {
            logger.LogError(ex, "No se pudo filtrar la biblioteca de Tracks");
            return new AppError(ErrorKind.Unavailable, "No se pudo leer la biblioteca.",
                "Comprueba la ruta de Tracks, los permisos y la disponibilidad del almacenamiento y vuelve a cargar.");
        }
    }
}
