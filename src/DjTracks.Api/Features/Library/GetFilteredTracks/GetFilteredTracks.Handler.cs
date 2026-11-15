using DjTracks.Domain;
using DjTracks.Infrastructure.Library;
using DjTracks.Infrastructure.Settings;
using FluentResults;

namespace DjTracks.Api.Features.Library.GetFilteredTracks;

public sealed class GetFilteredTracksHandler(PathSettings settings, TrackLibrary library,
    ILogger<GetFilteredTracksHandler> logger)
{
    public Result<TrackListing> Handle(GetFilteredTracksRequest request, CancellationToken ct)
    {
        try
        {
            var listing = library.Search(settings.Tracks,
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
