using DjTracks.Domain;
using DjTracks.UI.Shared;
using Microsoft.AspNetCore.Components;

namespace DjTracks.UI.Components.Layout;

public partial class SessionTracklistPanel
{
    [Parameter] public string? Url { get; set; }
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private ILogger<SessionTracklistPanel> Logger { get; set; } = default!;

    private string? _loadedUrl;
    private SessionTracklist? _tracklist;
    private string? _error;
    private bool _loading;
    private bool _disposed;
    private long _requestVersion;

    protected override Task OnParametersSetAsync() =>
        Url == _loadedUrl ? Task.CompletedTask : LoadAsync();

    private Task ReloadAsync() => LoadAsync();

    private async Task LoadAsync()
    {
        var version = ++_requestVersion;
        var url = _loadedUrl = Url;
        _tracklist = null;
        _error = null;
        _loading = url is not null;
        if (url is null)
            return;

        var result = await Http.GetForResultAsync<SessionTracklist>(url, Logger);
        // Descarta respuestas de una sesión anterior, incluso si se vuelve a ella.
        if (_disposed || version != _requestVersion)
            return;

        _loading = false;
        if (result.IsFailed)
            _error = result.Errors[0].Message;
        else
            _tracklist = result.Value;
    }

    public void Dispose()
    {
        _disposed = true;
        _requestVersion++;
    }
}
