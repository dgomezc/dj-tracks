using DjTracks.Domain;
using DjTracks.Infrastructure.Library;
using DjTracks.Infrastructure.Settings;
using FluentResults;

namespace DjTracks.Api.Features.Playlists.GetAutomaticPlaylistOptions;

public sealed class GetAutomaticPlaylistOptionsHandler(PathSettings settings, TrackLibrary library,
    ILogger<GetAutomaticPlaylistOptionsHandler> logger)
{
    public Result<AutomaticPlaylistOptions> Handle(CancellationToken ct)
    {
        try
        {
            return library.GetAutomaticOptions(settings.Tracks, ct);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException)
        {
            logger.LogError(ex, "No se pudieron cargar las opciones de playlists automáticas");
            return new AppError(ErrorKind.Unavailable, "No se pudo leer la biblioteca.",
                "Comprueba la ruta de Tracks, sus permisos y el almacenamiento y vuelve a cargar.");
        }
    }
}
