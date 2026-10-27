using System.Text.Json;
using DjTracks.Api.Shared;
using DjTracks.Domain;

namespace DjTracks.Api.Features.Streaming.GetTrackWaveform;

public sealed record GetTrackWaveformRequest(string? Year, string? Style, string? FileName);

public static class GetTrackWaveformEndpoint
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static void MapGetTrackWaveform(this RouteGroupBuilder api) =>
        api.MapGet("streaming/tracks/waveform", Handle).WithTags("Streaming");

    private static async Task<IResult> Handle([AsParameters] GetTrackWaveformRequest request,
        GetTrackWaveformHandler handler, HttpContext context, CancellationToken ct)
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

            var error = result.Errors[0].Message;
            await WriteChunkAsync(new TrackWaveformChunk(0, [], 0, 0,
                IsComplete: true, Error: error), ct);
        }

        return Results.Empty;
    }
}
