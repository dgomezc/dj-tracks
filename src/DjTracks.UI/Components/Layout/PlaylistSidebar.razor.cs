using DjTracks.Domain;
using DjTracks.UI.Shared;
using Microsoft.AspNetCore.Components;

namespace DjTracks.UI.Components.Layout;

public partial class PlaylistSidebar
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private ILogger<PlaylistSidebar> Logger { get; set; } = default!;
    [Inject] private PlaylistChanges Changes { get; set; } = default!;
    private ManualPlaylist[]? _playlists;
    private string? _error;
    private bool _busy;
    private int _version;

    protected override Task OnInitializedAsync()
    {
        Changes.Changed += OnChanged;
        return LoadAsync();
    }

    private async Task LoadAsync()
    {
        var version = ++_version;
        _busy = true;
        _error = null;
        var result = await Http.GetForResultAsync<ManualPlaylist[]>("api/playlists", Logger);
        if (version != _version)
            return;
        _busy = false;
        _playlists = result.IsSuccess ? result.Value : null;
        _error = result.IsFailed ? result.Errors[0].Message : null;
    }

    private void OnChanged() => _ = InvokeAsync(async () => { await LoadAsync(); StateHasChanged(); });

    public void Dispose()
    {
        _version++;
        Changes.Changed -= OnChanged;
    }
}
