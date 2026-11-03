using DjTracks.Domain;
using DjTracks.Infrastructure.Library;
using DjTracks.Infrastructure.Settings;
using FluentResults;

namespace DjTracks.Api.Features.Playlists.GetAutomaticPlaylistOptions;

public sealed class GetAutomaticPlaylistOptionsHandler(JsonSettingsStore settingsStore, TrackLibrary library,
    ILogger<GetAutomaticPlaylistOptionsHandler> logger)
{
    public async Task<Result<AutomaticPlaylistOptions>> HandleAsync(CancellationToken ct)
    {
        var settings = await settingsStore.LoadAsync(ct);
        if (settings.IsFailed)
            return settings.ToResult();
        if (string.IsNullOrWhiteSpace(settings.Value.Tracks))
            return new AppError(ErrorKind.Conflict, "Configura la ruta de Tracks.",
                "Abre Configuración y guarda las rutas antes de generar playlists.");
        try
        {
            return library.GetAutomaticOptions(settings.Value.Tracks, ct);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException)
        {
            logger.LogError(ex, "No se pudieron cargar las opciones de playlists automáticas");
            return new AppError(ErrorKind.Unavailable, "No se pudo leer la biblioteca.",
                "Comprueba la ruta de Tracks, sus permisos y el almacenamiento y vuelve a cargar.");
        }
    }
}
