using DjTracks.Domain;
using DjTracks.Infrastructure.Settings;
using FluentResults;

namespace DjTracks.Api.Features.Subsonic.GetMusicFolders;

public sealed class GetMusicFoldersHandler(JsonSettingsStore settingsStore)
{
    public async Task<Result<SubsonicFolder[]>> HandleAsync(CancellationToken ct)
    {
        var settings = await settingsStore.LoadAsync(ct);
        if (settings.IsFailed)
            return settings.ToResult();

        var folders = new[]
        {
            (Name: "Tracks", Path: settings.Value.Tracks),
            (Name: "Sessions", Path: settings.Value.Sessions),
            (Name: "Remember", Path: settings.Value.Remember)
        };
        if (folders.Any(folder => string.IsNullOrWhiteSpace(folder.Path)))
            return new AppError(ErrorKind.Conflict, "Bibliotecas sin configurar.",
                "Configura Tracks, Sessions y Remember antes de usar Subsonic.");
        return folders.Select(folder => new SubsonicFolder(
            new SubsonicId(folder.Name, []).Encode(), folder.Name)).ToArray();
    }
}

public sealed record SubsonicFolder(string Id, string Name);
