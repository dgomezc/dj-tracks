using DjTracks.Api.Shared;

namespace DjTracks.Api.Features.Library.GetTrackFilters;

public static class GetTrackFiltersEndpoint
{
    public static void MapGetTrackFilters(this RouteGroupBuilder api) =>
        api.MapGet("library/tracks/filter-options", Handle).WithTags("Library");

    private static IResult Handle(GetTrackFiltersHandler handler, CancellationToken ct) =>
        handler.Handle(ct).ToHttpResult();
}
