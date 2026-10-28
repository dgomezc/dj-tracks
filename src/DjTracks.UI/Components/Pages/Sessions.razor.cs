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

    private void Enqueue(TrackFile file)
    {
        if (_year is null || _session is null)
        {
            Logger.LogWarning("No se puede añadir {FileName}: falta año o carpeta de sesión", file.FileName);
            _error = "Vuelve a abrir la carpeta de la sesión antes de añadir su audio.";
            return;
        }

        var url = new Uri(Http.BaseAddress!, QueryHelpers.AddQueryString("api/streaming/sessions",
            new Dictionary<string, string?>
            {
                ["year"] = _year, ["session"] = _session, ["fileName"] = file.FileName
            })).AbsoluteUri;
        Player.Enqueue(url, null, null, file, _session);
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
