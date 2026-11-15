using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using DjTracks.Domain;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace DjTracks.Infrastructure.Library;

public sealed class SessionMarkStore(ILogger<SessionMarkStore> logger)
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web) { WriteIndented = true };
    private readonly SemaphoreSlim _gate = new(1, 1);

    public async Task<Result<SessionMark[]>> ReadAsync(string root, string year, string session,
        string fileName, CancellationToken ct) =>
        await ExecuteAsync(root, year, session, fileName, null, ct);

    public async Task<Result<SessionMark[]>> UpdateAsync(string root, string year, string session,
        string fileName, string line, double? seconds, CancellationToken ct) =>
        await ExecuteAsync(root, year, session, fileName, (line, seconds), ct);

    private async Task<Result<SessionMark[]>> ExecuteAsync(string root, string year, string session,
        string fileName, (string Line, double? Seconds)? update, CancellationToken ct)
    {
        await _gate.WaitAsync(ct);
        string? temporary = null;
        try
        {
            if (!Directory.Exists(root))
                return new AppError(ErrorKind.Unavailable, "Carpeta de marcas no disponible.",
                    "Comprueba session-marks dentro de Paths:Data, sus permisos y el montaje de datos de la API.");

            var key = JsonSerializer.Serialize(new[] { year, session, fileName });
            var name = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(key))).ToLowerInvariant();
            var path = Path.Combine(root, $"{name}.json");
            SessionMark[] marks = [];
            FileStream? input;
            try
            {
                if ((File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0)
                    throw new IOException("El fichero de marcas es un enlace.");
                input = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read,
                    4096, FileOptions.Asynchronous);
            }
            catch (FileNotFoundException) { input = null; }
            if (input is not null)
            {
                await using (input)
                {
                    var data = await JsonSerializer.DeserializeAsync<MarkFile>(input, Options, ct)
                        ?? throw new JsonException("Fichero de marcas vacío.");
                    if (data.Version != 1 || data.Year != year || data.Session != session || data.FileName != fileName
                        || data.Marks is null || data.Marks.Any(m => m is null || string.IsNullOrWhiteSpace(m.Line)
                            || !double.IsFinite(m.Seconds) || m.Seconds < 0
                            || m.Seconds >= TimeSpan.MaxValue.TotalSeconds - 1)
                        || data.Marks.Select(m => m.Line).Distinct(StringComparer.Ordinal).Count() != data.Marks.Length)
                        throw new JsonException("Fichero de marcas inválido.");
                    marks = data.Marks;
                }
            }

            if (update is not { } change)
                return marks;

            var list = marks.Where(mark => mark.Line != change.Line).ToList();
            if (change.Seconds is { } seconds)
                list.Add(new SessionMark(change.Line, seconds));
            marks = list.ToArray();
            temporary = $"{path}.{Guid.NewGuid():N}.tmp";
            await using (var output = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write,
                FileShare.None, 4096, FileOptions.Asynchronous))
            {
                await JsonSerializer.SerializeAsync(output, new MarkFile(1, year, session, fileName, marks), Options, ct);
                await output.FlushAsync(ct);
                output.Flush(flushToDisk: true);
            }
            ct.ThrowIfCancellationRequested();
            File.Move(temporary, path, overwrite: true);
            return marks;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            logger.LogError(ex, "No se pudo acceder a las marcas de {Year}/{Session}/{FileName}",
                year, session, fileName);
            return ex is JsonException
                ? new AppError(ErrorKind.Unexpected, "Fichero de marcas inválido.",
                    "No se han sobrescrito las marcas. Corrige o restaura el JSON y vuelve a cargar.")
                : new AppError(ErrorKind.Unavailable, "No se pudo acceder a las marcas.",
                    "Comprueba la carpeta de marcas, sus permisos y su disponibilidad.");
        }
        finally
        {
            try
            {
                if (temporary is not null && File.Exists(temporary))
                    File.Delete(temporary);
            }
            finally { _gate.Release(); }
        }
    }

    private sealed record MarkFile(int Version, string Year, string Session, string FileName, SessionMark[] Marks);
}
