using System.Text.Json;
using DjTracks.Api.Shared;
using DjTracks.Domain;

namespace DjTracks.Api.Features.Streaming.GetSessionWaveform;

public sealed record GetSessionWaveformRequest(string? Year, string? Session, string? FileName);

public static class GetSessionWaveformEndpoint
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static void MapGetSessionWaveform(this RouteGroupBuilder api) =>
        api.MapGet("streaming/sessions/waveform", Handle).WithTags("Streaming");

    private static async Task<IResult> Handle([AsParameters] GetSessionWaveformRequest request,
        GetSessionWaveformHandler handler, HttpContext context, CancellationToken ct)
    {
        async ValueTask WriteChunkAsync(TrackWaveformChunk chunk, CancellationToken token)
        {
            if (!context.Response.HasStarted)
            {
                context.Response.ContentType = "application/x-ndjson; charset=utf-8";
                context.Response.Headers.CacheControl = "no-store";
            }
            await JsonSerializer.SerializeAsync(context.Response.Body, chunk, JsonOptions, token);
            await context.Response.WriteAsync("\n", token);
            await context.Response.Body.FlushAsync(token);
        }

        var result = await handler.HandleAsync(request, WriteChunkAsync, ct);
        if (result.IsFailed)
        {
            if (!context.Response.HasStarted)
                return result.ToHttpResult();

            await WriteChunkAsync(new TrackWaveformChunk(0, [], 0, 0,
                IsComplete: true, Error: result.Errors[0].Message), ct);
        }

        return Results.Empty;
    }
}
