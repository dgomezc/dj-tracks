using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using DjTracks.Domain;
using FluentResults;
using FFMpegCore.Exceptions;
using Microsoft.Extensions.Logging;

namespace DjTracks.Infrastructure.Streaming;

public sealed class TrackWaveformCache(string cacheDirectory, IWaveformGenerator generator,
    ILogger<TrackWaveformCache> logger)
{
    private const int PeakCount = 1024;
    private const int CacheVersion = 1;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<Result<TrackWaveform>> GetAsync(FileInfo file, CancellationToken cancellationToken,
        Func<TrackWaveformChunk, CancellationToken, ValueTask>? onChunk = null)
    {
        file.Refresh();
        if (!file.Exists)
            return NotFound();

        var length = file.Length;
        var modifiedTicks = file.LastWriteTimeUtc.Ticks;
        var cachePath = GetCachePath(file.FullName);

        CacheEntry? cached = null;
        try
        {
            if (File.Exists(cachePath))
            {
                await using var cachedFile = new FileStream(cachePath, FileMode.Open, FileAccess.Read,
                    FileShare.Read | FileShare.Delete, 4096, FileOptions.Asynchronous);
                cached = await JsonSerializer.DeserializeAsync<CacheEntry>(
                    cachedFile, JsonOptions, cancellationToken);
            }
        }
        catch (JsonException ex)
        {
            logger.LogWarning(ex, "Se regenerará la caché de onda dañada para {FileName}", file.Name);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            logger.LogWarning(ex, "No se pudo leer la caché de onda para {FileName}", file.Name);
        }

        if (cached is { Version: CacheVersion, PeakCount: PeakCount }
            && cached.FileLength == length && cached.ModifiedTicks == modifiedTicks
            && cached.Peaks is { Length: PeakCount } && cached.DurationSeconds > 0)
        {
            if (onChunk is not null)
                await onChunk(new TrackWaveformChunk(0, cached.Peaks, PeakCount,
                    cached.DurationSeconds, IsComplete: true), cancellationToken);
            return Result.Ok(new TrackWaveform(cached.DurationSeconds, cached.Peaks));
        }

        try
        {
            var waveform = await generator.GenerateAsync(file, PeakCount, onChunk, cancellationToken);
            file.Refresh();
            if (!file.Exists || file.Length != length || file.LastWriteTimeUtc.Ticks != modifiedTicks)
                return new AppError(ErrorKind.Conflict, "La canción ha cambiado.",
                    "El fichero cambió mientras se generaba la forma de onda. Vuelve a cargar la pista.");

            Directory.CreateDirectory(cacheDirectory);
            var temporaryPath = $"{cachePath}.{Guid.NewGuid():N}.tmp";
            try
            {
                await using (var stream = new FileStream(temporaryPath, FileMode.CreateNew,
                    FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous))
                {
                    await JsonSerializer.SerializeAsync(stream,
                        new CacheEntry(CacheVersion, length, modifiedTicks,
                            PeakCount, waveform.DurationSeconds, waveform.Peaks), JsonOptions, cancellationToken);
                    await stream.FlushAsync(cancellationToken);
                }

                File.Move(temporaryPath, cachePath, overwrite: true);
            }
            finally
            {
                if (File.Exists(temporaryPath))
                    File.Delete(temporaryPath);
            }

            if (onChunk is not null)
                await onChunk(new TrackWaveformChunk(PeakCount, [], PeakCount,
                    waveform.DurationSeconds, IsComplete: true), cancellationToken);

            return Result.Ok(waveform);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException
            or InvalidDataException or TagLib.CorruptFileException
            or TagLib.UnsupportedFormatException or FFMpegException or System.ComponentModel.Win32Exception
            || ex.InnerException is System.ComponentModel.Win32Exception)
        {
            logger.LogError(ex, "No se pudo generar la forma de onda de {FileName}", file.Name);
            return new AppError(ErrorKind.Unavailable, "No se pudo generar la forma de onda.",
                "Comprueba que el MP3 sea legible y que FFmpeg esté instalado y disponible para la API.");
        }
    }

    private string GetCachePath(string filePath)
    {
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(filePath)));
        return Path.Combine(cacheDirectory, $"{hash}.json");
    }

    private static AppError NotFound() =>
        new(ErrorKind.NotFound, "Canción no encontrada.",
            "El fichero seleccionado ya no existe. Vuelve a cargar la biblioteca.");

    private sealed record CacheEntry(int Version, long FileLength, long ModifiedTicks, int PeakCount,
        double DurationSeconds, float[] Peaks);
}
