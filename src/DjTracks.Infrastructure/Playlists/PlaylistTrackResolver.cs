using DjTracks.Domain;
using DjTracks.Infrastructure.Library;
using DjTracks.Infrastructure.Settings;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace DjTracks.Infrastructure.Playlists;

public sealed class PlaylistTrackResolver(JsonSettingsStore settingsStore, TrackLibrary tracks,
    RememberLibrary remember, ILogger<PlaylistTrackResolver> logger)
{
    public async Task<Result<ResolvedPlaylistTrack[]>> ResolveAsync(
        IEnumerable<PlaylistTrackReference> references, CancellationToken ct)
    {
        var settings = await settingsStore.LoadAsync(ct);
        if (settings.IsFailed)
            return settings.ToResult();
        List<ResolvedPlaylistTrack> resolved = [];
        try
        {
            foreach (var reference in references)
            {
                ct.ThrowIfCancellationRequested();
                var root = reference.Library == "Tracks" ? settings.Value.Tracks : settings.Value.Remember;
                if (string.IsNullOrWhiteSpace(root))
                    return new AppError(ErrorKind.Conflict, "Configura la biblioteca.",
                        "Abre Configuración y guarda las rutas antes de añadir o cargar canciones.");
                var result = reference.Library == "Tracks"
                    ? tracks.ResolveTrackFile(root, reference.Year!, reference.Style!, reference.FileName, ct)
                    : remember.ResolveAudioFile(root, reference.Category!, reference.FileName, ct);
                if (result.IsFailed)
                {
                    logger.LogWarning("No se encontró la canción {Library}/{FileName} de una playlist",
                        reference.Library, reference.FileName);
                    return new AppError(ErrorKind.NotFound, "Canción no disponible.",
                        $"No se encontró «{reference.FileName}» ({reference.Library}). Restaura el archivo o quítalo de la playlist y vuelve a cargar. La cola no se ha modificado.");
                }
                var details = TrackLibrary.ReadTrack(result.Value, ct);
                if (details.Warning is not null)
                    logger.LogWarning("No se pudieron leer los tags de {Library}/{FileName}",
                        reference.Library, reference.FileName);
                if (reference.Library == "Remember")
                    details = details with { Year = null, Style = null };
                resolved.Add(new(reference, details));
            }
            return resolved.ToArray();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException)
        {
            logger.LogError(ex, "No se pudieron resolver las canciones de una playlist");
            return new AppError(ErrorKind.Unavailable, "Biblioteca no disponible.",
                "Comprueba las rutas, los permisos y el almacenamiento de música. La cola no se ha modificado.");
        }
    }
}
