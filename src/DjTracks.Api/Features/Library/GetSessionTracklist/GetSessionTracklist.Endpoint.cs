using DjTracks.Api.Shared;

namespace DjTracks.Api.Features.Library.GetSessionTracklist;

public sealed record GetSessionTracklistRequest(string? Year, string? Session, string? FileName);

public static class GetSessionTracklistEndpoint
{
    public static void MapGetSessionTracklist(this RouteGroupBuilder api) =>
        api.MapGet("library/sessions/tracklist", Handle).WithTags("Library");

    private static async Task<IResult> Handle([AsParameters] GetSessionTracklistRequest request,
        GetSessionTracklistHandler handler, CancellationToken ct) =>
        (await handler.HandleAsync(request, ct)).ToHttpResult();
}
