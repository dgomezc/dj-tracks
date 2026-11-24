using DjTracks.Api.Shared;

namespace DjTracks.Api.Features.Library.GetSessions;

public sealed record GetSessionsRequest(string? Year, string? Session);

public static class GetSessionsEndpoint
{
    public static void MapGetSessions(this RouteGroupBuilder api) =>
        api.MapGet("library/sessions", Handle).WithTags("Library");

    private static IResult Handle([AsParameters] GetSessionsRequest request,
        GetSessionsHandler handler, CancellationToken ct) =>
        handler.Handle(request, ct).ToHttpResult();
}
