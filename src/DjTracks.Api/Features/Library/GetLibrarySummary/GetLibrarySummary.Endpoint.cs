using DjTracks.Api.Shared;

namespace DjTracks.Api.Features.Library.GetLibrarySummary;

public sealed record GetLibrarySummaryRequest(string? Library);

public static class GetLibrarySummaryEndpoint
{
    public static void MapGetLibrarySummary(this RouteGroupBuilder api) =>
        api.MapGet("library/summary", Handle).WithTags("Library");

    private static async Task<IResult> Handle([AsParameters] GetLibrarySummaryRequest request,
        GetLibrarySummaryHandler handler, CancellationToken ct) =>
        (await handler.HandleAsync(request, ct)).ToHttpResult();
}
