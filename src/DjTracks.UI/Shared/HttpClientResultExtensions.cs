using System.Text.Json;
using DjTracks.Domain;
using FluentResults;

namespace DjTracks.UI.Shared;

public static class HttpClientResultExtensions
{
    public static Task<Result<T>> GetForResultAsync<T>(this HttpClient http, string url, ILogger logger,
        CancellationToken ct = default) =>
        SendAsync<T>(() => http.GetAsync(url, ct), url, logger, ct);

    public static Task<Result<T>> PutForResultAsync<TBody, T>(this HttpClient http, string url, TBody body,
        ILogger logger, CancellationToken ct = default) =>
        SendAsync<T>(() => http.PutAsJsonAsync(url, body, ct), url, logger, ct);

    public static Task<Result<T>> PostForResultAsync<TBody, T>(this HttpClient http, string url, TBody body,
        ILogger logger, CancellationToken ct = default) =>
        SendAsync<T>(() => http.PostAsJsonAsync(url, body, ct), url, logger, ct);

    public static Task<Result<T>> DeleteForResultAsync<T>(this HttpClient http, string url,
        ILogger logger, CancellationToken ct = default) =>
        SendAsync<T>(() => http.DeleteAsync(url, ct), url, logger, ct);

    private static async Task<Result<T>> SendAsync<T>(Func<Task<HttpResponseMessage>> send, string url,
        ILogger logger, CancellationToken ct)
    {
        try
        {
            using var response = await send();
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<T>(ct)
                    ?? throw new JsonException("Respuesta vacía de la API.");

            logger.LogWarning("La API devolvió HTTP {StatusCode} en {Url}", (int)response.StatusCode, url);
            var problem = await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>(ct);
            var title = problem?.Title ?? "Error de la API.";
            return new AppError(ErrorKind.Unexpected, title,
                problem?.Detail ?? problem?.Title ?? $"La API devolvió un error ({(int)response.StatusCode}).",
                problem?.Errors);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException
            or JsonException or NotSupportedException)
        {
            logger.LogError(ex, "Fallo al llamar a la API en {Url}", url);
            return ex is JsonException or NotSupportedException
                ? new AppError(ErrorKind.Unexpected, "Respuesta no válida.",
                    "La API devolvió una respuesta no válida. Revisa el servidor y vuelve a cargar.")
                : new AppError(ErrorKind.Unavailable, "API no disponible.",
                    "No se pudo contactar con la API o se agotó el tiempo de espera. Comprueba que está iniciada y vuelve a cargar.");
        }
    }
}
