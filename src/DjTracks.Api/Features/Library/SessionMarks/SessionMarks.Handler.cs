using System.Text;
using DjTracks.Domain;
using DjTracks.Infrastructure.Library;
using DjTracks.Infrastructure.Settings;
using FluentResults;

namespace DjTracks.Api.Features.Library.SessionMarks;

public sealed class SessionMarksHandler(PathSettings settings, SessionLibrary library,
    SessionMarkStore store, ILogger<SessionMarksHandler> logger)
{
    public Task<Result<SessionMark[]>> GetAsync(GetSessionMarksRequest request, CancellationToken ct) =>
        store.ReadAsync(settings.SessionMarks, request.Year!, request.Session!, request.FileName!, ct);

    public async Task<Result<SessionMark[]>> SaveAsync(SaveSessionMarkRequest request, CancellationToken ct)
    {
        try
        {
            var tracklist = await library.ReadTracklistAsync(settings.Sessions, request.Year!,
                request.Session!, request.FileName!, ct);
            if (tracklist.IsFailed) return tracklist.ToResult();
            var lines = (tracklist.Value.Content ?? "").Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            if (lines.Count(line => line == request.Line) != 1)
                return new AppError(ErrorKind.Conflict, "No se puede guardar la marca.",
                    "La línea ha cambiado o está duplicada en el TXT. Vuelve a cargar el tracklist.");
            return await store.UpdateAsync(settings.SessionMarks, request.Year!, request.Session!,
                request.FileName!, request.Line!, request.Seconds, ct);
        }
        catch (DecoderFallbackException ex)
        {
            logger.LogWarning(ex, "Codificación no válida del tracklist de {Year}/{Session}", request.Year, request.Session);
            return new AppError(ErrorKind.Validation, "No se pudo leer el tracklist.",
                "Guarda el TXT como UTF-8 y vuelve a cargar.");
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException)
        {
            logger.LogError(ex, "No se pudo comprobar el tracklist de {Year}/{Session}", request.Year, request.Session);
            return new AppError(ErrorKind.Unavailable, "No se pudo comprobar el tracklist.",
                "Comprueba la ruta de Sessions y vuelve a cargar.");
        }
    }
}
