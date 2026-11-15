using DjTracks.Domain;
using DjTracks.UI.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DjTracks.UI.Components.Layout;

public partial class SessionTracklistPanel
{
    [Parameter] public string? Url { get; set; }
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private AudioPlayerState Player { get; set; } = default!;
    [Inject] private IJSRuntime JS { get; set; } = default!;
    [Inject] private ILogger<SessionTracklistPanel> Logger { get; set; } = default!;

    private string? _loadedUrl;
    private SessionTracklist? _tracklist;
    private SessionMark[] _marks = [];
    private string[] _lines = [];
    private HashSet<string> _duplicates = new(StringComparer.Ordinal);
    private string? _error;
    private string? _marksError;
    private bool _loading;
    private bool _saving;
    private bool _marksLoaded;
    private bool _disposed;
    private long _requestVersion;
    private IJSObjectReference? _module;

    private string[] Lines => _lines;
    private bool IsDuplicate(string line) => _duplicates.Contains(line);
    private bool HasDuplicates => _duplicates.Count > 0;
    private int OrphanCount => _marks.Count(mark => !Lines.Contains(mark.Line, StringComparer.Ordinal));

    protected override Task OnParametersSetAsync() =>
        Url == _loadedUrl ? Task.CompletedTask : LoadAsync();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _module = await JS.InvokeAsync<IJSObjectReference>("import",
                "./Components/Layout/SessionTracklistPanel.razor.js");
            StateHasChanged();
        }
    }

    private Task ReloadAsync() => LoadAsync();

    private async Task LoadAsync()
    {
        var version = ++_requestVersion;
        var url = _loadedUrl = Url;
        var current = Player.Current;
        _tracklist = null;
        _lines = [];
        _duplicates.Clear();
        _marks = [];
        _marksLoaded = false;
        _marksError = null;
        _error = null;
        _loading = url is not null;
        Player.SetSessionMarks(url, []);
        if (url is null || current?.SessionFolder is null)
            return;

        var result = await Http.GetForResultAsync<SessionTracklist>(url, Logger);
        if (_disposed || version != _requestVersion) return;
        _loading = false;
        if (result.IsFailed)
        {
            _error = result.Errors[0].Message;
            return;
        }
        _tracklist = result.Value;
        if (string.IsNullOrWhiteSpace(_tracklist.Content)) return;
        _lines = _tracklist.Content.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
        _duplicates = _lines.Where(line => !string.IsNullOrWhiteSpace(line))
            .GroupBy(line => line, StringComparer.Ordinal).Where(group => group.Count() > 1)
            .Select(group => group.Key).ToHashSet(StringComparer.Ordinal);

        var marksUrl = $"api/library/sessions/marks?year={Uri.EscapeDataString(current.Details.Year!)}"
            + $"&session={Uri.EscapeDataString(current.SessionFolder)}&fileName={Uri.EscapeDataString(current.Details.FileName)}";
        var marks = await Http.GetForResultAsync<SessionMark[]>(marksUrl, Logger);
        if (_disposed || version != _requestVersion) return;
        if (marks.IsFailed) _marksError = marks.Errors[0].Message;
        else
        {
            _marks = marks.Value;
            _marksLoaded = true;
            PublishMarks(url);
        }
    }

    private async Task SaveAsync(string line, bool delete)
    {
        var current = Player.Current;
        if (_module is null || current?.SessionFolder is null || !_marksLoaded || _saving) return;
        var version = Player.PlaybackVersion;
        var requestVersion = _requestVersion;
        _saving = true;
        _marksError = null;
        try
        {
            double? seconds = null;
            if (!delete)
            {
                seconds = await _module.InvokeAsync<double?>("getPosition", version);
                if (seconds is null || version != Player.PlaybackVersion)
                {
                    _marksError = "El audio no está listo o ha cambiado. Vuelve a intentarlo.";
                    return;
                }
            }
            var body = new { Year = current.Details.Year, Session = current.SessionFolder,
                current.Details.FileName, Line = line, Seconds = seconds };
            var result = await Http.PutForResultAsync<object, SessionMark[]>("api/library/sessions/marks", body, Logger);
            if (_disposed || requestVersion != _requestVersion) return;
            if (result.IsFailed) _marksError = result.Errors[0].Message;
            else
            {
                _marks = result.Value;
                PublishMarks(_loadedUrl);
            }
        }
        finally { _saving = false; }
    }

    private void PublishMarks(string? url) => Player.SetSessionMarks(url,
        _marks.Where(mark => !_duplicates.Contains(mark.Line)
            && _lines.Contains(mark.Line, StringComparer.Ordinal)));

    private async Task SeekAsync(double seconds)
    {
        if (_module is null) return;
        var success = await _module.InvokeAsync<bool>("seek", Player.PlaybackVersion, seconds);
        if (!success) _marksError = "El audio no está listo o la marca supera su duración.";
    }

    private static string FormatTime(double seconds)
    {
        var time = TimeSpan.FromSeconds(seconds);
        return time.TotalHours >= 1 ? $"{(int)time.TotalHours}:{time.Minutes:00}:{time.Seconds:00}"
            : $"{(int)time.TotalMinutes}:{time.Seconds:00}";
    }

    public async ValueTask DisposeAsync()
    {
        _disposed = true;
        _requestVersion++;
        if (_module is null) return;
        try { await _module.DisposeAsync(); }
        catch (JSDisconnectedException) { Logger.LogDebug("Circuito desconectado al liberar las marcas"); }
    }
}
