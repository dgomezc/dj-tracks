using DjTracks.Domain;
using DjTracks.Infrastructure.Library;
using DjTracks.Infrastructure.Settings;
using FluentResults;

namespace DjTracks.Api.Features.Library.GetLibrarySummary;

public sealed class GetLibrarySummaryHandler(JsonSettingsStore settingsStore, TrackLibrary tracks,
    RememberLibrary remember, ILogger<GetLibrarySummaryHandler> logger)
{
    public async Task<Result<LibrarySummary>> HandleAsync(GetLibrarySummaryRequest request, CancellationToken ct)
    {
        var settings = await settingsStore.LoadAsync(ct);
        if (settings.IsFailed)
            return settings.ToResult();

        var root = request.Library switch
        {
            "Tracks" => settings.Value.Tracks,
            "Sessions" => settings.Value.Sessions,
            _ => settings.Value.Remember
        };
        if (string.IsNullOrWhiteSpace(root))
            return new AppError(ErrorKind.Conflict, "Biblioteca no configurada.",
                $"Configura la ruta de {request.Library} para ver su resumen.");

        try
        {
            if (request.Library == "Remember")
            {
                var references = remember.IndexReferences(root, ct);
                return references.IsFailed ? references.ToResult()
                    : new LibrarySummary(request.Library, references.Value.Length);
            }

            return new LibrarySummary(request.Library!, tracks.IndexReferences(root, ct).Length);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException
            or ArgumentException or NotSupportedException)
        {
            logger.LogWarning(ex, "No se pudo contar la biblioteca {Library}", request.Library);
            return new AppError(ErrorKind.Unavailable, "Biblioteca no disponible.",
                $"No se pudo leer {request.Library}. Comprueba su ruta, permisos y almacenamiento.");
        }
    }
}
