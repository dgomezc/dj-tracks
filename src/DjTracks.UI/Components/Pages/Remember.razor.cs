using DjTracks.Domain;
using DjTracks.UI.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace DjTracks.UI.Components.Pages;

public partial class Remember
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private ILogger<Remember> Logger { get; set; } = default!;
    [Inject] private AudioPlayerState Player { get; set; } = default!;

    private static readonly string[] Categories = ["Cortas", "Largas"];
    private string _category = "Cortas";
    private RememberListing? _listing;
    private string? _error;
    private bool _busy;

    protected override Task OnInitializedAsync() => ReloadAsync();

    private Task ReloadAsync() => LoadAsync(_category);

    private async Task LoadAsync(string category)
    {
        if (_busy)
            return;
        _category = category;
        _busy = true;
        _error = null;
        _listing = null;
        try
        {
            var result = await Http.GetForResultAsync<RememberListing>(
                QueryHelpers.AddQueryString("api/library/remember", "category", category), Logger);
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

    private void PlayRemember(TrackFile file, bool enqueue = false)
    {
        var query = new Dictionary<string, string?> { ["category"] = _category, ["fileName"] = file.FileName };
        var audioUrl = new Uri(Http.BaseAddress!, QueryHelpers.AddQueryString("api/streaming/remember", query)).AbsoluteUri;
        var waveformUrl = new Uri(Http.BaseAddress!, QueryHelpers.AddQueryString("api/streaming/remember/waveform", query)).AbsoluteUri;
        var artworkUrl = new Uri(Http.BaseAddress!, QueryHelpers.AddQueryString("api/streaming/remember/artwork", query)).AbsoluteUri;
        if (enqueue)
            Player.Enqueue(audioUrl, waveformUrl, artworkUrl, file, rememberCategory: _category);
        else
            Player.Play(audioUrl, waveformUrl, artworkUrl, file, rememberCategory: _category);
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
