using DjTracks.Api.Shared;

namespace DjTracks.Api.Features.Stats.GetPlaybackStatistics;

public sealed record GetPlaybackStatisticsRequest(int? Days);

public static class GetPlaybackStatisticsEndpoint
{
    public static void MapGetPlaybackStatistics(this RouteGroupBuilder api) =>
        api.MapGet("stats/dashboard", Handle).WithTags("Stats");

    private static async Task<IResult> Handle([AsParameters] GetPlaybackStatisticsRequest request,
        GetPlaybackStatisticsHandler handler, CancellationToken ct) =>
        (await handler.HandleAsync(request, ct)).ToHttpResult();
}
