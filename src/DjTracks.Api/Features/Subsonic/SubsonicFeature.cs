using DjTracks.Api.Features.Subsonic.GetIndexes;
using DjTracks.Api.Features.Subsonic.GetMusicDirectory;
using DjTracks.Api.Features.Subsonic.GetMusicFolders;
using DjTracks.Api.Features.Subsonic.GetPlaylists;
using DjTracks.Api.Features.Subsonic.Stream;
using DjTracks.Infrastructure.Streaming;

namespace DjTracks.Api.Features.Subsonic;

public static class SubsonicFeature
{
    public static IServiceCollection AddSubsonicFeature(this IServiceCollection services) =>
        services.AddScoped<GetMusicFoldersHandler>()
            .AddScoped<GetMusicDirectoryHandler>()
            .AddScoped<GetIndexesHandler>()
            .AddScoped<GetPlaylistsHandler>()
            .AddScoped<SubsonicStreamHandler>()
            .AddSingleton(sp => new SubsonicTranscoder(
                sp.GetRequiredService<IConfiguration>()["Waveform:BinaryFolder"]));

    public static void MapSubsonicFeature(this WebApplication app)
    {
        app.MapGet("/rest/getOpenSubsonicExtensions", Extensions);
        app.MapGet("/rest/getOpenSubsonicExtensions.view", Extensions);
        var rest = app.MapGroup("/rest").AddEndpointFilter(SubsonicAuthentication.FilterAsync);
        rest.MapGet("/ping", Ping);
        rest.MapGet("/ping.view", Ping);
        rest.MapGetMusicFolders();
        rest.MapGetIndexes();
        rest.MapGetMusicDirectory();
        rest.MapGetPlaylists();
        rest.MapSubsonicStream();
        rest.MapGet("/{method}", Unsupported);
    }

    private static IResult Ping(HttpContext context) => SubsonicResponse.Ok(context);

    private static IResult Extensions(HttpContext context) =>
        SubsonicResponse.Ok(context, "openSubsonicExtensions", Array.Empty<object>());

    private static IResult Unsupported(HttpContext context) =>
        SubsonicResponse.Error(context, 50, "Operación no admitida.");
}
