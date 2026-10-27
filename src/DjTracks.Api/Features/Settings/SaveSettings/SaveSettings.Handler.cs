using DjTracks.Domain;
using DjTracks.Infrastructure.Settings;
using FluentResults;

namespace DjTracks.Api.Features.Settings.SaveSettings;

public sealed class SaveSettingsHandler(JsonSettingsStore settingsStore)
{
    public async Task<Result<PathSettings>> HandleAsync(SaveSettingsRequest request, CancellationToken ct)
    {
        var settings = new PathSettings
        {
            Tracks = request.Tracks!.Trim(),
            Sessions = request.Sessions!.Trim(),
            Remember = request.Remember!.Trim(),
            Playlists = request.Playlists!.Trim(),
            M3uRoot = request.M3uRoot!.Trim()
        };

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
            var error = CheckFolder(path, writable: name == nameof(settings.Playlists));
            if (error is not null)
                errors[name] = [error];
        }
        if (errors.Count > 0)
            return new AppError(ErrorKind.Validation, "Revisa las rutas indicadas.",
                "Revisa las rutas indicadas.", errors);

        return (await settingsStore.SaveAsync(settings, ct)).ToResult(settings);
    }

    private static string? CheckFolder(string path, bool writable)
    {
        try
        {
            using var entries = Directory.EnumerateFileSystemEntries(path).GetEnumerator();
            entries.MoveNext();
            if (writable)
            {
                using var probe = new FileStream(Path.Combine(path, $".djtracks-{Guid.NewGuid():N}.tmp"),
                    FileMode.CreateNew, FileAccess.Write, FileShare.None, 1, FileOptions.DeleteOnClose);
            }
            return null;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException
            or NotSupportedException)
        {
            return writable
                ? "La carpeta debe existir y permitir lectura y escritura al proceso de la API."
                : "La carpeta no existe o el proceso de la API no puede leerla.";
        }
    }
}
