using DjTracks.Domain;
using DjTracks.UI.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DjTracks.UI.Components.Layout;

public partial class BottomPlayer
{
    [Inject] private AudioPlayerState Player { get; set; } = default!;
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private IJSRuntime JS { get; set; } = default!;
    [Inject] private ILogger<BottomPlayer> Logger { get; set; } = default!;
    [Inject] private IConfiguration Configuration { get; set; } = default!;
    [Parameter] public bool QueueOpen { get; set; }
    [Parameter] public EventCallback OnToggleQueue { get; set; }

    private ElementReference _root;
    private ElementReference _audio;
    private AddToPlaylist _addToPlaylist = default!;
    private IJSObjectReference? _module;
    private DotNetObjectReference<BottomPlayer>? _reference;
    private long _attachedVersion = -1;
    private bool _paused = true;
    private bool _muted;
    private bool _disposed;
    private bool _waveformUnavailable;
    private double _volume = 70;
    private double _lastAudibleVolume = 70;
    private string? _playbackError;
    private string? _statisticsError;
    private PlaybackRecord? _playbackRecord;
    private long _recordVersion = -1;
    private bool IsSilent => _muted || _volume == 0;
    private string BrowserAudioUrl(string audioUrl) =>
        Configuration.GetValue<bool>("StreamingProxy:Enabled")
            ? new Uri(audioUrl, UriKind.Absolute).PathAndQuery
            : audioUrl;
    private string RepeatLabel => Player.Repeat switch
    {
        QueueRepeatMode.All => "Bucle: cola. Cambiar a canción",
        QueueRepeatMode.One => "Bucle: canción. Desactivar",
        _ => "Bucle desactivado. Activar bucle de cola"
    };

    private PlaylistTrackReference? PlaylistReference => Player.Current is { SessionFolder: null } current
        ? current.RememberCategory is { } category
            ? new("Remember", current.Details.FileName, Category: category)
            : new("Tracks", current.Details.FileName, current.Details.Year, current.Details.Style)
        : null;

    protected override void OnInitialized()
    {
        Player.Changed += OnPlayerChanged;
        UpdatePlaybackRecord();
    }

    private void OnPlayerChanged()
    {
        if (_attachedVersion != Player.PlaybackVersion)
        {
            _paused = true;
            _playbackError = null;
            _waveformUnavailable = false;
        }
        UpdatePlaybackRecord();
        _ = InvokeAsync(StateHasChanged);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _module = await JS.InvokeAsync<IJSObjectReference>("import", "./Components/Layout/BottomPlayer.razor.js");
            _reference = DotNetObjectReference.Create(this);
            await _module.InvokeVoidAsync("observePlayer", _root);
            StateHasChanged();
        }

        if (_module is null || _attachedVersion == Player.PlaybackVersion)
            return;
        _attachedVersion = Player.PlaybackVersion;
        await _module.InvokeVoidAsync("attachPlayer", _root, Player.Current is null ? null : _audio,
            _reference, _attachedVersion, _volume / 100, _muted, _playbackRecord);
    }

    private async Task TogglePlaybackAsync()
    {
        if (_module is not null && Player.Current is not null)
            await _module.InvokeVoidAsync("togglePlayback", _root);
    }

    private async Task SetVolumeAsync(double value)
    {
        _volume = Math.Clamp(value, 0, 100);
        if (_volume > 0)
            _lastAudibleVolume = _volume;
        _muted = false;
        await ApplyVolumeAsync();
    }

    private async Task ToggleMuteAsync()
    {
        if (IsSilent)
        {
            _muted = false;
            if (_volume == 0)
                _volume = _lastAudibleVolume;
        }
        else
            _muted = true;
        await ApplyVolumeAsync();
    }

    private async Task ApplyVolumeAsync()
    {
        if (_module is not null && Player.Current is not null)
            await _module.InvokeVoidAsync("setVolume", _root, _volume / 100, _muted);
    }

    private Task AddToPlaylistAsync() => PlaylistReference is { } track
        ? _addToPlaylist.ShowAsync(track) : Task.CompletedTask;

    private void OnWaveformUnavailable(bool unavailable) => _waveformUnavailable = unavailable;

    [JSInvokable]
    public Task OnPlaybackState(long version, bool paused, string? error) => InvokeAsync(() =>
    {
        if (_disposed || version != Player.PlaybackVersion)
            return;
        _paused = paused;
        _playbackError = error;
        if (error is not null)
            Logger.LogWarning("Fallo de reproducción de {Title}: {Error}", Player.Title, error);
        StateHasChanged();
    });

    [JSInvokable]
    public Task OnPlaybackEnded(long version) => InvokeAsync(() =>
    {
        if (!_disposed)
            Player.OnEnded(version);
    });

    [JSInvokable]
    public Task OnPlaybackThreshold(long version, PlaybackRecord record) => InvokeAsync(async () =>
    {
        if (_disposed || record.EventId == Guid.Empty)
            return;

        var result = await Http.PostForResultAsync<object, Guid>("api/stats/plays",
            new { playback = record }, Logger);
        if (result.IsFailed)
        {
            Logger.LogWarning("No se pudo registrar la reproducción de {FileName}: {Error}",
                record.FileName, result.Errors[0].Message);
            if (version == Player.PlaybackVersion)
                _statisticsError = "No se pudo guardar esta reproducción en las estadísticas. El audio continúa.";
        }
        else if (version == Player.PlaybackVersion)
            _statisticsError = null;

        if (!_disposed && version == Player.PlaybackVersion)
            StateHasChanged();
    });

    private void UpdatePlaybackRecord()
    {
        if (_recordVersion == Player.PlaybackVersion)
            return;
        _recordVersion = Player.PlaybackVersion;
        _statisticsError = null;
        if (Player.Current is not { } current)
        {
            _playbackRecord = null;
            return;
        }

        var details = current.Details;
        _playbackRecord = current.SessionFolder is { } session
            ? new PlaybackRecord(Guid.NewGuid(), "Sessions", details.FileName,
                Year: details.Year, SessionFolder: session, Title: current.Title, Artists: details.Artists)
            : current.RememberCategory is { } category
                ? new PlaybackRecord(Guid.NewGuid(), "Remember", details.FileName,
                    Category: category, Title: current.Title, Artists: details.Artists)
                : new PlaybackRecord(Guid.NewGuid(), "Tracks", details.FileName,
                    Year: details.Year, Style: details.Style, Title: current.Title, Artists: details.Artists);
    }

    public async ValueTask DisposeAsync()
    {
        _disposed = true;
        Player.Changed -= OnPlayerChanged;
        try
        {
            if (_module is not null)
            {
                await _module.InvokeVoidAsync("disposePlayer", _root);
                await _module.DisposeAsync();
            }
        }
        catch (JSDisconnectedException)
        {
            Logger.LogDebug("Circuito desconectado al liberar el reproductor");
        }
        finally
        {
            _reference?.Dispose();
        }
    }
}
