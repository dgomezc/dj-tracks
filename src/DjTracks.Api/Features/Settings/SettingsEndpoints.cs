using System.Text.Json;
using DjTracks.Domain;
using DjTracks.Infrastructure.Settings;

namespace DjTracks.Api.Features.Settings;

public static class SettingsEndpoints
{
    public static void MapSettingsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/settings").WithTags("Settings");
        group.MapGet("/", (JsonSettingsStore store, ILoggerFactory loggerFactory, CancellationToken ct) =>
            WithStorageErrors(async () => Results.Ok(await store.LoadAsync(ct)), loggerFactory));
        group.MapPut("/", (PathSettings settings, JsonSettingsStore store,
            ILoggerFactory loggerFactory, CancellationToken ct) =>
            WithStorageErrors(async () =>
            {
                var normalized = settings with
                {
                    Tracks = settings.Tracks?.Trim() ?? "",
                    Sessions = settings.Sessions?.Trim() ?? "",
                    Remember = settings.Remember?.Trim() ?? "",
                    Playlists = settings.Playlists?.Trim() ?? "",
                    M3uRoot = settings.M3uRoot?.Trim() ?? ""
                };
                var errors = Validate(normalized);
                if (errors.Count > 0)
                    return Results.ValidationProblem(errors, title: "Revisa las rutas indicadas.");

                await store.SaveAsync(normalized, ct);
                return Results.Ok(normalized);
            }, loggerFactory));
    }

    private static Dictionary<string, string[]> Validate(PathSettings settings)
    {
        Dictionary<string, string[]> errors = [];
        foreach (var (name, path) in new[]
        {
            (nameof(settings.Tracks), settings.Tracks),
            (nameof(settings.Sessions), settings.Sessions),
            (nameof(settings.Remember), settings.Remember),
            (nameof(settings.Playlists), settings.Playlists),
            (nameof(settings.M3uRoot), settings.M3uRoot)
        })
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                errors[name] = ["Introduce una ruta."];
                continue;
            }
            if (!Path.IsPathFullyQualified(path))
            {
                errors[name] = ["Introduce una ruta absoluta del servidor (local o de red)."];
                continue;
            }

            try
            {
                using var entries = Directory.EnumerateFileSystemEntries(path).GetEnumerator();
                entries.MoveNext();
                if (name == nameof(settings.Playlists))
                {
                    using var probe = new FileStream(Path.Combine(path, $".djtracks-{Guid.NewGuid():N}.tmp"),
                        FileMode.CreateNew, FileAccess.Write, FileShare.None, 1, FileOptions.DeleteOnClose);
                }
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException
                or NotSupportedException)
            {
                errors[name] = [name == nameof(settings.Playlists)
                    ? "La carpeta debe existir y permitir lectura y escritura al proceso de la API."
                    : "La carpeta no existe o el proceso de la API no puede leerla."];
            }
        }
        return errors;
    }

    private static async Task<IResult> WithStorageErrors(Func<Task<IResult>> action,
        ILoggerFactory loggerFactory)
    {
        try
        {
            return await action();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            loggerFactory.CreateLogger("Settings").LogError(ex, "Error al acceder al JSON de ajustes");
            return Results.Problem(statusCode: 500, title: "No se pudo acceder a los ajustes.",
                detail: ex is JsonException
                    ? "El JSON de ajustes es inválido. Corrígelo o restaura una copia en el servidor y vuelve a cargar. No se ha sobrescrito."
                    : "Comprueba los permisos y la disponibilidad del fichero de ajustes en el servidor. No se ha confirmado ningún guardado.");
        }
    }
}
