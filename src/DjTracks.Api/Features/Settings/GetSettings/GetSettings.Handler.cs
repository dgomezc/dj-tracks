using DjTracks.Domain;
using DjTracks.Infrastructure.Settings;
using FluentResults;

namespace DjTracks.Api.Features.Settings.GetSettings;

public sealed class GetSettingsHandler(JsonSettingsStore settingsStore)
{
    public Task<Result<PathSettings>> HandleAsync(CancellationToken ct) => settingsStore.LoadAsync(ct);
}
