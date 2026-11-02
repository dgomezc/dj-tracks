using DjTracks.Domain;
using DjTracks.UI.Shared;
using Microsoft.AspNetCore.Components;

namespace DjTracks.UI.Components.Layout;

public partial class PlaylistDialog
{
    [Parameter] public bool Open { get; set; }
    [Parameter] public EventCallback<bool> OpenChanged { get; set; }
    [Parameter] public ManualPlaylist? Playlist { get; set; }
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private ILogger<PlaylistDialog> Logger { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private PlaylistChanges Changes { get; set; } = default!;
    private bool _wasOpen;
    private bool _busy;
    private string? _name;
    private string? _error;

    protected override void OnParametersSet()
    {
        if (Open && !_wasOpen)
        {
            _name = Playlist?.Name;
            _error = null;
        }
        _wasOpen = Open;
    }

    private Task OnOpenChanged(bool open) => open ? Task.CompletedTask : CloseAsync();

    private Task CloseAsync() => _busy ? Task.CompletedTask : OpenChanged.InvokeAsync(false);

    private async Task SaveAsync()
    {
        if (_busy)
            return;
        _busy = true;
        _error = null;
        var renaming = Playlist;
        var body = new { Name = _name };
        var result = renaming is not null
            ? await Http.PutForResultAsync<object, ManualPlaylist>($"api/playlists/{renaming.Id}", body, Logger)
            : await Http.PostForResultAsync<object, ManualPlaylist>("api/playlists", body, Logger);
        _busy = false;
        if (result.IsFailed)
        {
            _error = result.Errors[0].Message;
            return;
        }
        Changes.Notify();
        await OpenChanged.InvokeAsync(false);
        if (renaming is not null)
            Changes.NotifyRenamed(result.Value);
        else
            Navigation.NavigateTo($"/playlists/{result.Value.Id}");
    }
}