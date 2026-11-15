using DjTracks.Api.Shared;

namespace DjTracks.Api.Features.Library.SessionMarks;

public sealed record GetSessionMarksRequest(string? Year, string? Session, string? FileName);
public sealed record SaveSessionMarkRequest(string? Year, string? Session, string? FileName, string? Line, double? Seconds);

public static class SessionMarksEndpoint
{
    public static void MapSessionMarks(this RouteGroupBuilder api)
    {
        api.MapGet("library/sessions/marks", Get).WithTags("Library");
        api.MapPut("library/sessions/marks", Save).WithTags("Library");
    }

    private static async Task<IResult> Get([AsParameters] GetSessionMarksRequest request,
        SessionMarksHandler handler, CancellationToken ct) =>
        (await handler.GetAsync(request, ct)).ToHttpResult();

    private static async Task<IResult> Save(SaveSessionMarkRequest request,
        SessionMarksHandler handler, CancellationToken ct) =>
        (await handler.SaveAsync(request, ct)).ToHttpResult();
}
