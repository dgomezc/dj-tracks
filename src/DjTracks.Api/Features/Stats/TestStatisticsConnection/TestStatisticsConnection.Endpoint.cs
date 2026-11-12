using DjTracks.Api.Shared;

namespace DjTracks.Api.Features.Stats.TestStatisticsConnection;

public static class TestStatisticsConnectionEndpoint
{
    public static void MapTestStatisticsConnection(this RouteGroupBuilder api) =>
        api.MapGet("stats/connection", Handle).WithTags("Stats");

    private static async Task<IResult> Handle(TestStatisticsConnectionHandler handler, CancellationToken ct) =>
        (await handler.HandleAsync(ct)).ToHttpResult();
}
