using DjTracks.Api.Shared;

namespace DjTracks.Api.Features.Library.GetSessions;

public sealed record GetSessionsRequest(string? Year, string? Session);

public static class GetSessionsEndpoint
{
    public static void MapGetSessions(this RouteGroupBuilder api) =>
        api.MapGet("library/sessions", Handle).WithTags("Library");

    private static async Task<IResult> Handle([AsParameters] GetSessionsRequest request,
        GetSessionsHandler handler, CancellationToken ct) =>
        (await handler.HandleAsync(request, ct)).ToHttpResult();
}
