using BlazorBlueprint.Primitives;
using DjTracks.Domain;
using DjTracks.UI.Shared;
using Microsoft.AspNetCore.Components;

namespace DjTracks.UI.Components.Layout;

public partial class AutomaticPlaylistDialog
{
    [Parameter] public bool Open { get; set; }
    [Parameter] public EventCallback<bool> OpenChanged { get; set; }
    [Parameter] public ManualPlaylist? Playlist { get; set; }
    [Parameter] public EventCallback<ManualPlaylist> Saved { get; set; }
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private ILogger<AutomaticPlaylistDialog> Logger { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private PlaylistChanges Changes { get; set; } = default!;
    private bool _wasOpen;
    private bool _busy;
    private bool _loading;
    private int _version;
    private string? _name;
    private string? _error;
    private IEnumerable<string>? _years;
    private IEnumerable<string>? _styles;
    private bool _includeRemember;
    private AutomaticPlaylistOptions? _options;
    private IEnumerable<SelectOption<string>> YearOptions => ToOptions(_options?.Years, _years);
    private IEnumerable<SelectOption<string>> StyleOptions => ToOptions(_options?.Styles, _styles);

    protected override async Task OnParametersSetAsync()
    {
        var opening = Open && !_wasOpen;
        _wasOpen = Open;
        if (!opening)
            return;
        _name = Playlist?.Name;
        _years = Playlist?.Filters?.Years.ToArray();
        _styles = Playlist?.Filters?.Styles.ToArray();
        _includeRemember = Playlist?.Filters?.IncludeRemember ?? false;
        _options = null;
        await LoadOptionsAsync();
    }

    private static IEnumerable<SelectOption<string>> ToOptions(string[]? available, IEnumerable<string>? selected) =>
        (available ?? []).Concat(selected ?? []).Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(value => new SelectOption<string>(value, value));

    private async Task LoadOptionsAsync()
    {
        var version = ++_version;
        _loading = true;
        _error = null;
        var result = await Http.GetForResultAsync<AutomaticPlaylistOptions>("api/playlists/automatic/options", Logger);
        if (version != _version)
            return;
        _loading = false;
        _options = result.IsSuccess ? result.Value : null;
        _error = result.IsFailed ? result.Errors[0].Message : null;
    }

    private Task OnOpenChanged(bool open) => open ? Task.CompletedTask : CloseAsync();

    private Task CloseAsync()
    {
        if (_busy)
            return Task.CompletedTask;
        _version++;
        return OpenChanged.InvokeAsync(false);
    }

    private async Task SaveAsync()
    {
        if (_busy || _loading || _options is null)
            return;
        _busy = true;
        _error = null;
        var editing = Playlist;
        var body = new { Name = _name, Filters = new AutomaticPlaylistFilters(
            _years?.ToArray() ?? [], _styles?.ToArray() ?? [], _includeRemember) };
        var result = editing is null
            ? await Http.PostForResultAsync<object, ManualPlaylist>("api/playlists/automatic", body, Logger)
            : await Http.PutForResultAsync<object, ManualPlaylist>($"api/playlists/{editing.Id}/automatic", body, Logger);
        _busy = false;
        if (result.IsFailed)
        {
            _error = result.Errors[0].Message;
            return;
        }
        Changes.Notify();
        await OpenChanged.InvokeAsync(false);
        await Saved.InvokeAsync(result.Value);
        if (editing is null)
            Navigation.NavigateTo($"/playlists/{result.Value.Id}");
    }
}
