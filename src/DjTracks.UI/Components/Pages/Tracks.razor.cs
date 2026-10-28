using DjTracks.Domain;
using DjTracks.UI.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace DjTracks.UI.Components.Pages;

public partial class Tracks
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private ILogger<Tracks> Logger { get; set; } = default!;
    [Inject] private AudioPlayerState AudioPlayer { get; set; } = default!;

    private TrackListing? _listing;
    private string? _year;
    private string? _style;
    private string? _error;
    private bool _busy;
    private bool _filtersOpen;
    private TrackFilterOptions? _filterOptions;
    private IReadOnlyCollection<string> _filterYears = [];
    private IReadOnlyCollection<string> _filterGroups = [];
    private IReadOnlyCollection<string> _filterSubstyles = [];
    private TrackFilter? _appliedFilter;

    protected override Task OnInitializedAsync() => LoadAsync(null, null);

    private Task OpenFolderAsync(string folder) =>
        _year is null ? LoadAsync(folder, null) : LoadAsync(_year, folder);

    private Task LoadAsync(string? year, string? style)
    {
        if (_busy)
            return Task.CompletedTask;
        _appliedFilter = null;
        _year = year;
        _style = style;
        return RequestAsync();
    }

    private Task ApplyFiltersAsync()
    {
        if (_busy)
            return Task.CompletedTask;
        _appliedFilter = new(_filterYears.ToArray(), _filterGroups.ToArray(),
            _filterSubstyles.ToArray());
        _year = null;
        _style = null;
        return RequestAsync();
    }

    private Task ClearFiltersAsync()
    {
        if (_busy)
            return Task.CompletedTask;
        _filterYears = _filterGroups = _filterSubstyles = [];
        return LoadAsync(null, null);
    }

    private async Task OnFiltersExpandedAsync(bool expanded)
    {
        _filtersOpen = expanded;
        if (expanded && !_busy)
        {
            _busy = true;
            _error = null;
            try
            {
                await LoadFilterOptionsAsync();
            }
            finally
            {
                _busy = false;
            }
        }
    }

    private Task ReloadAsync() => RequestAsync();

    private void PlayTrack(TrackFile track, bool enqueue = false)
    {
        var year = track.Year ?? _year;
        var style = track.Style ?? _style;
        if (string.IsNullOrWhiteSpace(year) || string.IsNullOrWhiteSpace(style))
        {
            Logger.LogWarning("No se puede reproducir {FileName}: faltan el año o el estilo",
                track.FileName);
            _error = "No se puede reproducir esta canción porque faltan los datos de su carpeta.";
            return;
        }

        var query = new Dictionary<string, string?>
        {
            ["year"] = year,
            ["style"] = style,
            ["fileName"] = track.FileName
        };
        var audioUrl = new Uri(Http.BaseAddress!,
            QueryHelpers.AddQueryString("api/streaming/tracks", query)).AbsoluteUri;
        var waveformUrl = new Uri(Http.BaseAddress!,
            QueryHelpers.AddQueryString("api/streaming/tracks/waveform", query)).AbsoluteUri;
        var artworkUrl = new Uri(Http.BaseAddress!,
            QueryHelpers.AddQueryString("api/streaming/tracks/artwork", query)).AbsoluteUri;
        var details = track with { Year = year, Style = style };
        if (enqueue)
            AudioPlayer.Enqueue(audioUrl, waveformUrl, artworkUrl, details);
        else
            AudioPlayer.Play(audioUrl, waveformUrl, artworkUrl, details);
    }

    private async Task RequestAsync()
    {
        if (_busy)
            return;
        _busy = true;
        _error = null;
        _listing = null;
        try
        {
            if (_filtersOpen && !await LoadFilterOptionsAsync())
                return;

            var url = _appliedFilter is null ? "api/library/tracks" : "api/library/tracks/search";
            List<string> query = [];
            if (_appliedFilter is null)
            {
                AddQuery(query, "year", _year);
                AddQuery(query, "style", _style);
            }
            else
            {
                foreach (var year in _appliedFilter.Years)
                    AddQuery(query, "year", year);
                foreach (var group in _appliedFilter.Groups)
                    AddQuery(query, "group", group);
                foreach (var substyle in _appliedFilter.Substyles)
                    AddQuery(query, "substyle", substyle);
            }
            if (query.Count > 0)
                url += "?" + string.Join("&", query);

            var result = await Http.GetForResultAsync<TrackListing>(url, Logger);
            if (result.IsFailed)
            {
                _error = result.Errors[0].Message;
                return;
            }
            _listing = result.Value;
        }
        finally
        {
            _busy = false;
        }
    }

    private async Task<bool> LoadFilterOptionsAsync()
    {
        var result = await Http.GetForResultAsync<TrackFilterOptions>(
            "api/library/tracks/filter-options", Logger);
        if (result.IsFailed)
        {
            _error = result.Errors[0].Message;
            _filterOptions = null;
            return false;
        }
        _filterOptions = result.Value;
        return true;
    }

    private static void AddQuery(List<string> query, string name, string? value)
    {
        if (value is not null)
            query.Add($"{name}={Uri.EscapeDataString(value)}");
    }

    private string FilterSummary => _appliedFilter is null ? ""
        : $"{SelectionSummary(_appliedFilter.Years, "Todos los años")} / {SelectionSummary(_appliedFilter.Groups, "Todos los estilos")} / {SelectionSummary(_appliedFilter.Substyles, "Todos los subestilos")}";

    private static string SelectionSummary(string[] values, string emptyText) =>
        values.Length == 0 ? emptyText : string.Join(", ", values);

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
