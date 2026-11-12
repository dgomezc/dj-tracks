using DjTracks.Infrastructure.Library;
using DjTracks.Infrastructure.Settings;

namespace DjTracks.Api.Features.Playlists;

public sealed class PlaylistLibraryIndexWorker(PlaylistLibraryIndex index, JsonSettingsStore settingsStore)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await RefreshAsync(force: true, stoppingToken);
        await Task.WhenAll(WatchAsync(stoppingToken), RescanHourlyAsync(stoppingToken));
    }

    private async Task WatchAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            await index.WaitForChangesAsync(ct);
            await RefreshAsync(force: false, ct);
        }
    }

    private async Task RescanHourlyAsync(CancellationToken ct)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromHours(1));
        while (await timer.WaitForNextTickAsync(ct))
            await RefreshAsync(force: true, ct);
    }

    private async Task RefreshAsync(bool force, CancellationToken ct)
    {
        var settings = await settingsStore.LoadAsync(ct);
        if (settings.IsSuccess)
            await index.RefreshAsync(settings.Value, force, ct);
    }
}
