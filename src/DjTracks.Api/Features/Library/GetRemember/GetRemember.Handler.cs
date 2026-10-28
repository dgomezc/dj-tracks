using DjTracks.Domain;
using DjTracks.Infrastructure.Library;
using DjTracks.Infrastructure.Settings;
using FluentResults;

namespace DjTracks.Api.Features.Library.GetRemember;

public sealed class GetRememberHandler(JsonSettingsStore settingsStore, RememberLibrary library,
    ILogger<GetRememberHandler> logger)
{
    public async Task<Result<RememberListing>> HandleAsync(GetRememberRequest request, CancellationToken ct)
    {
        var settings = await settingsStore.LoadAsync(ct);
        if (settings.IsFailed)
            return settings.ToResult();
        if (string.IsNullOrWhiteSpace(settings.Value.Remember))
            return new AppError(ErrorKind.Conflict, "Configura la ruta de Remember.",
                "Abre Configuración y guarda las rutas antes de explorar Remember.");

        try
        {
            var result = library.Browse(settings.Value.Remember, request.Category!, ct);
            if (result.IsSuccess)
                foreach (var file in result.Value.Files.Where(file => file.Warning is not null))
                    logger.LogWarning("No se pudieron leer los tags de Remember/{Category}/{FileName}",
                        request.Category, file.FileName);
            return result;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException)
        {
            logger.LogError(ex, "No se pudo explorar la biblioteca de Remember");
            return new AppError(ErrorKind.Unavailable, "No se pudo leer Remember.",
                "Comprueba la ruta de Remember, los permisos y la disponibilidad del almacenamiento y vuelve a cargar.");
        }
    }
}
