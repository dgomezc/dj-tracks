using DjTracks.Api.Shared;

namespace DjTracks.Api.Features.Library.GetRemember;

public sealed record GetRememberRequest(string? Category);

public static class GetRememberEndpoint
{
    public static void MapGetRemember(this RouteGroupBuilder api) =>
        api.MapGet("library/remember", Handle).WithTags("Library");

    private static async Task<IResult> Handle([AsParameters] GetRememberRequest request,
        GetRememberHandler handler, CancellationToken ct) =>
        (await handler.HandleAsync(request, ct)).ToHttpResult();
}
