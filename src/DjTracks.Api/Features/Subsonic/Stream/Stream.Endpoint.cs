using DjTracks.Infrastructure.Streaming;
using FluentValidation;
using Microsoft.Net.Http.Headers;

namespace DjTracks.Api.Features.Subsonic.Stream;

public static class SubsonicStreamEndpoint
{
    public static void MapSubsonicStream(this RouteGroupBuilder rest)
    {
        rest.MapGet("/stream", Stream);
        rest.MapGet("/stream.view", Stream);
        rest.MapGet("/download", Download);
        rest.MapGet("/download.view", Download);
    }

    private static Task<IResult> Download(HttpContext context, SubsonicStreamHandler handler,
        IValidator<GetSubsonicAudioRequest> validator, CancellationToken ct) =>
        Handle(context, handler, validator, null, true, ct);

    private static Task<IResult> Stream(HttpContext context, SubsonicStreamHandler handler,
        IValidator<GetSubsonicAudioRequest> validator, SubsonicTranscoder transcoder, CancellationToken ct) =>
        Handle(context, handler, validator, transcoder, false, ct);

    private static async Task<IResult> Handle(HttpContext context, SubsonicStreamHandler handler,
        IValidator<GetSubsonicAudioRequest> validator, SubsonicTranscoder? transcoder,
        bool download, CancellationToken ct)
    {
        var request = new GetSubsonicAudioRequest(context.Request.Query["id"]);
        if (!(await validator.ValidateAsync(request, ct)).IsValid)
            return SubsonicResponse.Error(context, 70, "Canción no encontrada.");
        SubsonicId.TryParse(request.Id, out var id);

        string? codec = null;
        string? container = null;
        string? contentType = null;
        int? bitrate = null;
        if (!download)
        {
            var query = context.Request.Query;
            if (query.TryGetValue("timeOffset", out var offset) && offset != "0")
                return SubsonicResponse.Error(context, 0, "No se admite timeOffset para audio.");
            if (query.TryGetValue("maxBitRate", out var rate)
                && (!int.TryParse(rate, out var parsed) || parsed < 0 || parsed > 320))
                return SubsonicResponse.Error(context, 0, "El bitrate debe estar entre 0 y 320 kbps.");
            if (query.TryGetValue("maxBitRate", out rate) && int.TryParse(rate, out var value) && value > 0)
                bitrate = value;
            var format = query["format"].ToString().ToLowerInvariant();
            switch (format)
            {
                case "":
                case "mp3":
                    if (bitrate is not null)
                        (codec, container, contentType) = ("libmp3lame", "mp3", "audio/mpeg");
                    break;
                case "raw":
                    break;
                case "opus":
                    (codec, container, contentType) = ("libopus", "opus", "audio/ogg");
                    break;
                case "ogg":
                    (codec, container, contentType) = ("libvorbis", "ogg", "audio/ogg");
                    break;
                case "aac":
                    (codec, container, contentType) = ("aac", "adts", "audio/aac");
                    break;
                default:
                    return SubsonicResponse.Error(context, 0, "Formato de transcodificación no admitido.");
            }
            if (codec is not null && context.Request.Headers.TryGetValue(HeaderNames.Range, out var range)
                && range != "bytes=0-")
                return Results.StatusCode(StatusCodes.Status416RangeNotSatisfiable);
        }

        var result = handler.Handle(id, ct);
        if (result.IsFailed)
            return SubsonicResponse.FromError(context, result);
        context.Response.Headers.CacheControl = "no-store";
        if (codec is null)
            return Results.File(result.Value, "audio/mpeg",
                fileDownloadName: download ? id.Parts[^1] : null, enableRangeProcessing: true);

        return Results.Stream(async output =>
        {
            await using var input = result.Value;
            try
            {
                await transcoder!.TranscodeAsync(input, output, codec, container!, bitrate, ct);
            }
            catch (Exception ex) when (ex is IOException or FFMpegCore.Exceptions.FFMpegException)
            {
                context.RequestServices.GetRequiredService<ILoggerFactory>()
                    .CreateLogger("Subsonic.Transcoding")
                    .LogError(ex, "Falló la transcodificación Subsonic.");
                context.Abort();
            }
        }, contentType);
    }
}
