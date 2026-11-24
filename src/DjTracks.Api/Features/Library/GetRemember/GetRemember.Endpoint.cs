using DjTracks.Api.Shared;

namespace DjTracks.Api.Features.Library.GetRemember;

public sealed record GetRememberRequest(string? Category);

public static class GetRememberEndpoint
{
    public static void MapGetRemember(this RouteGroupBuilder api) =>
        api.MapGet("library/remember", Handle).WithTags("Library");

    private static IResult Handle([AsParameters] GetRememberRequest request,
        GetRememberHandler handler, CancellationToken ct) =>
        handler.Handle(request, ct).ToHttpResult();
}
