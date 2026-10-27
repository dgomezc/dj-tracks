using DjTracks.Domain;
using DjTracks.UI.Shared;
using Microsoft.AspNetCore.Components;

namespace DjTracks.UI.Components.Pages;

public partial class Tracks
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private ILogger<Tracks> Logger { get; set; } = default!;

    private TrackListing? _listing;
    private string? _year;
    private string? _style;
    private string? _error;
    private bool _busy;

    protected override Task OnInitializedAsync() => LoadAsync(null, null);

    private Task OpenFolderAsync(string folder) =>
        _year is null ? LoadAsync(folder, null) : LoadAsync(_year, folder);

    private async Task LoadAsync(string? year, string? style)
    {
        if (_busy)
            return;
        _busy = true;
        _error = null;
        _listing = null;
        _year = year;
        _style = style;
        try
        {
            var url = "api/library/tracks";
            if (year is not null)
                url += $"?year={Uri.EscapeDataString(year)}";
            if (style is not null)
                url += $"&style={Uri.EscapeDataString(style)}";

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
