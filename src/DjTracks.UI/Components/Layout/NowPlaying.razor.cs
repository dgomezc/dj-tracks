using DjTracks.Domain;
using DjTracks.UI.Shared;
using Microsoft.AspNetCore.Components;

namespace DjTracks.UI.Components.Layout;

public partial class NowPlaying
{
    [Inject] private AudioPlayerState Player { get; set; } = default!;
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private ILogger<NowPlaying> Logger { get; set; } = default!;

    private string? _artworkUrl;
    private string? _dataUrl;
    private string? _error;
    private bool _loading;
    private bool _disposed;
    private long _requestVersion;

    protected override Task OnInitializedAsync()
    {
        Player.Changed += OnPlayerChanged;
        return RefreshAsync();
    }

    private void OnPlayerChanged() => _ = InvokeAsync(() => RefreshAsync());

    private Task ReloadAsync() => RefreshAsync(force: true);

    private async Task RefreshAsync(bool force = false)
    {
        if (_disposed)
            return;

        var url = Player.Current?.ArtworkUrl;
        if (!force && url == _artworkUrl)
        {
            StateHasChanged();
            return;
        }

        _artworkUrl = url;
        var version = ++_requestVersion;
        _dataUrl = null;
        _error = null;
        _loading = url is not null;
        StateHasChanged();
        if (url is null)
            return;

        var result = await Http.GetForResultAsync<TrackArtwork>(url, Logger);
        // Una respuesta atrasada no debe pintar la carátula de la pista anterior.
        if (_disposed || version != _requestVersion)
            return;

        _loading = false;
        if (result.IsFailed)
            _error = result.Errors[0].Message;
        else
            _dataUrl = result.Value.DataUrl;
        StateHasChanged();
    }

    private void OnImageError(string dataUrl)
    {
        if (dataUrl != _dataUrl)
            return;
        Logger.LogWarning("El navegador no pudo mostrar la carátula de {Title}", Player.Title);
        _error = "La imagen incrustada no se pudo mostrar. Comprueba la carátula del MP3.";
    }

    private static string Display(string? value) =>
        string.IsNullOrWhiteSpace(value) ? "Sin datos" : value;

    private static string Duration(double? seconds)
    {
        if (seconds is null)
            return "Sin datos";
        var duration = TimeSpan.FromSeconds(seconds.Value);
        return duration.TotalHours >= 1
            ? $"{(int)duration.TotalHours}:{duration.Minutes:00}:{duration.Seconds:00}"
            : $"{(int)duration.TotalMinutes}:{duration.Seconds:00}";
    }

    public void Dispose()
    {
        _disposed = true;
        Player.Changed -= OnPlayerChanged;
    }
}
