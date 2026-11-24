using DjTracks.Domain;
using DjTracks.Infrastructure.Library;
using DjTracks.Infrastructure.Settings;
using FluentResults;

namespace DjTracks.Api.Features.Library.GetLibrarySummary;

public sealed class GetLibrarySummaryHandler(PathSettings settings, TrackLibrary tracks,
    RememberLibrary remember, ILogger<GetLibrarySummaryHandler> logger)
{
    public Result<LibrarySummary> Handle(GetLibrarySummaryRequest request, CancellationToken ct)
    {
        var root = request.Library switch
        {
            "Tracks" => settings.Tracks,
            "Sessions" => settings.Sessions,
            _ => settings.Remember
        };

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
