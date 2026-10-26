using System.Text.Json;
using DjTracks.Domain;

namespace DjTracks.Infrastructure.Settings;

public sealed class JsonSettingsStore(string filePath)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    private readonly SemaphoreSlim _gate = new(1, 1);

    public async Task<PathSettings> LoadAsync(CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            return await ReadAsync(cancellationToken);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task SaveAsync(PathSettings settings, CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken);
        string? temporaryPath = null;
        try
        {
            // No sobrescribir un fichero corrupto aunque el formulario se cargara antes.
            await ReadAsync(cancellationToken);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            temporaryPath = $"{filePath}.{Guid.NewGuid():N}.tmp";
            await using (var stream = new FileStream(temporaryPath, FileMode.CreateNew,
                FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous))
            {
                await JsonSerializer.SerializeAsync(stream, settings, JsonOptions, cancellationToken);
                await stream.FlushAsync(cancellationToken);
                stream.Flush(flushToDisk: true);
            }

            cancellationToken.ThrowIfCancellationRequested();
            if (File.Exists(filePath))
                File.Replace(temporaryPath, filePath, null);
            else
                File.Move(temporaryPath, filePath);
        }
        finally
        {
            try
            {
                if (temporaryPath is not null && File.Exists(temporaryPath))
                    File.Delete(temporaryPath);
            }
            finally
            {
                _gate.Release();
            }
        }
    }

    private async Task<PathSettings> ReadAsync(CancellationToken cancellationToken)
    {
        FileStream stream;
        try
        {
            stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }
        catch (FileNotFoundException)
        {
            return new PathSettings();
        }
        catch (DirectoryNotFoundException)
        {
            return new PathSettings();
        }

        await using (stream)
        {
            var settings = await JsonSerializer.DeserializeAsync<PathSettings>(
                stream, JsonOptions, cancellationToken);
            if (settings is null || new[] { settings.Tracks, settings.Sessions, settings.Remember,
                    settings.Playlists, settings.M3uRoot }
                .Any(path => string.IsNullOrWhiteSpace(path) || !Path.IsPathFullyQualified(path)))
            {
                throw new JsonException("El JSON de ajustes debe contener las cinco rutas absolutas.");
            }

            return settings;
        }
    }
}
