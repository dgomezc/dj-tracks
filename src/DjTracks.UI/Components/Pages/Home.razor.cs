using BlazorBlueprint.Primitives;
using DjTracks.Domain;
using DjTracks.UI.Shared;
using Microsoft.AspNetCore.Components;

namespace DjTracks.UI.Components.Pages;

public partial class Home
{
    private sealed class LibraryCard(string name, string url)
    {
        public string Name { get; } = name;
        public string Url { get; } = url;
        public int? Count { get; set; }
        public string? Error { get; set; }
    }

    private static readonly SelectOption<int>[] PeriodOptions =
    [
        new(7, "Últimos 7 días"),
        new(30, "Últimos 30 días"),
        new(0, "Todo el historial")
    ];

    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private ILogger<Home> Logger { get; set; } = default!;

    private readonly LibraryCard[] _libraries =
    [
        new("Tracks", "/tracks"),
        new("Sessions", "/sessions"),
        new("Remember", "/remember")
    ];
    private int _days = 30;
    private bool _loadingStatistics;
    private bool _loadingSummary;
    private PlaybackStatistics? _statistics;
    private PlaylistSummary? _playlists;
    private string? _statisticsError;
    private string? _playlistsError;

    protected override Task OnInitializedAsync() => ReloadAsync();

    private Task ReloadAsync() => Task.WhenAll(LoadStatisticsAsync(), LoadSummaryAsync());

    private async Task ChangePeriodAsync(int days)
    {
        if (_loadingStatistics || days == _days)
            return;
        _days = days;
        await LoadStatisticsAsync();
    }

    private async Task LoadStatisticsAsync()
    {
        _loadingStatistics = true;
        _statistics = null;
        _statisticsError = null;
        try
        {
            var url = _days == 0 ? "api/stats/dashboard" : $"api/stats/dashboard?days={_days}";
            var result = await Http.GetForResultAsync<PlaybackStatistics>(url, Logger);
            if (result.IsFailed)
                _statisticsError = result.Errors[0].Message;
            else
                _statistics = result.Value;
        }
        finally
        {
            _loadingStatistics = false;
        }
    }

    private async Task LoadSummaryAsync()
    {
        _loadingSummary = true;
        _playlists = null;
        _playlistsError = null;
        foreach (var library in _libraries)
        {
            library.Count = null;
            library.Error = null;
        }
        try
        {
            await Task.WhenAll(_libraries.Select(LoadLibraryAsync).Append(LoadPlaylistsAsync()));
        }
        finally
        {
            _loadingSummary = false;
        }
    }

    private async Task LoadLibraryAsync(LibraryCard library)
    {
        var result = await Http.GetForResultAsync<LibrarySummary>(
            $"api/library/summary?library={library.Name}", Logger);
        if (result.IsFailed)
            library.Error = result.Errors[0].Message;
        else
            library.Count = result.Value.AudioFiles;
    }

    private async Task LoadPlaylistsAsync()
    {
        var result = await Http.GetForResultAsync<PlaylistSummary>("api/playlists/summary", Logger);
        if (result.IsFailed)
            _playlistsError = result.Errors[0].Message;
        else
            _playlists = result.Value;
    }

    private static string Title(PlaybackHistoryEntry entry) =>
        string.IsNullOrWhiteSpace(entry.Title) ? entry.FileName : entry.Title;

    private static string Artist(PlaybackHistoryEntry entry) =>
        string.IsNullOrWhiteSpace(entry.Artist) ? "Sin artista" : entry.Artist;

    private static string Date(DateTimeOffset date) => date.UtcDateTime.ToString("dd/MM/yyyy HH:mm") + " UTC";
}
