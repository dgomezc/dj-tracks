using DjTracks.Api.Shared;

namespace DjTracks.Api.Features.Streaming.GetTrackWaveform;

public sealed record GetTrackWaveformRequest(string? Year, string? Style, string? FileName);

public static class GetTrackWaveformEndpoint
{
    public static void MapGetTrackWaveform(this RouteGroupBuilder api) =>
        api.MapGet("streaming/tracks/waveform", Handle).WithTags("Streaming");

    private static async Task<IResult> Handle([AsParameters] GetTrackWaveformRequest request,
        GetTrackWaveformHandler handler, CancellationToken ct) =>
        (await handler.HandleAsync(request, ct)).ToHttpResult();
}
