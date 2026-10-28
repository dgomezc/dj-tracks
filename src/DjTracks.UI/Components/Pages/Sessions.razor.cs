using DjTracks.Domain;
using DjTracks.UI.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace DjTracks.UI.Components.Pages;

public partial class Sessions
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private ILogger<Sessions> Logger { get; set; } = default!;
    [Inject] private AudioPlayerState Player { get; set; } = default!;

    private SessionListing? _listing;
    private string? _year;
    private string? _session;
    private string? _error;
    private bool _busy;

    protected override Task OnInitializedAsync() => LoadAsync(null, null);

    private Task OpenFolderAsync(string folder) =>
        _year is null ? LoadAsync(folder, null) : LoadAsync(_year, folder);

    private Task ReloadAsync() => LoadAsync(_year, _session);

    private async Task LoadAsync(string? year, string? session)
    {
        if (_busy)
            return;
        _year = year;
        _session = session;
        _busy = true;
        _error = null;
        _listing = null;
        try
        {
            var url = QueryHelpers.AddQueryString("api/library/sessions",
                new Dictionary<string, string?> { ["year"] = year, ["session"] = session });
            var result = await Http.GetForResultAsync<SessionListing>(url, Logger);
            if (result.IsFailed)
                _error = result.Errors[0].Message;
            else
                _listing = result.Value;
        }
        finally
        {
            _busy = false;
        }
    }

    private void PlaySession(TrackFile file, bool enqueue = false)
    {
        if (_year is null || _session is null)
        {
            Logger.LogWarning("No se puede reproducir {FileName}: falta año o carpeta de sesión", file.FileName);
            _error = "Vuelve a abrir la carpeta de la sesión antes de reproducir o añadir su audio.";
            return;
        }

        var query = new Dictionary<string, string?>
        {
            ["year"] = _year, ["session"] = _session, ["fileName"] = file.FileName
        };
        var audioUrl = new Uri(Http.BaseAddress!,
            QueryHelpers.AddQueryString("api/streaming/sessions", query)).AbsoluteUri;
        var waveformUrl = new Uri(Http.BaseAddress!,
            QueryHelpers.AddQueryString("api/streaming/sessions/waveform", query)).AbsoluteUri;
        var artworkUrl = new Uri(Http.BaseAddress!,
            QueryHelpers.AddQueryString("api/streaming/sessions/artwork", query)).AbsoluteUri;
        var tracklistUrl = new Uri(Http.BaseAddress!,
            QueryHelpers.AddQueryString("api/library/sessions/tracklist", query)).AbsoluteUri;
        if (enqueue)
            Player.Enqueue(audioUrl, waveformUrl, artworkUrl, file, _session, tracklistUrl);
        else
            Player.Play(audioUrl, waveformUrl, artworkUrl, file, _session, tracklistUrl);
    }

    private static string Display(string? value) => string.IsNullOrWhiteSpace(value) ? "Sin datos" : value;

    private static string Duration(double? seconds)
    {
        if (seconds is null)
            return "Sin datos";
        var duration = TimeSpan.FromSeconds(seconds.Value);
        return duration.TotalHours >= 1
            ? $"{(int)duration.TotalHours}:{duration.Minutes:00}:{duration.Seconds:00}"
            : $"{(int)duration.TotalMinutes}:{duration.Seconds:00}";
    }
}
