using DjTracks.Api.Shared;

namespace DjTracks.Api.Features.Library.GetLibrarySummary;

public sealed record GetLibrarySummaryRequest(string? Library);

public static class GetLibrarySummaryEndpoint
{
    public static void MapGetLibrarySummary(this RouteGroupBuilder api) =>
        api.MapGet("library/summary", Handle).WithTags("Library");

    private static IResult Handle([AsParameters] GetLibrarySummaryRequest request,
        GetLibrarySummaryHandler handler, CancellationToken ct) =>
        handler.Handle(request, ct).ToHttpResult();
}
