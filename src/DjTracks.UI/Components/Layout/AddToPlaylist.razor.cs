using BlazorBlueprint.Primitives;
using DjTracks.Domain;
using DjTracks.UI.Shared;
using Microsoft.AspNetCore.Components;

namespace DjTracks.UI.Components.Layout;

public partial class AddToPlaylist
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private ILogger<AddToPlaylist> Logger { get; set; } = default!;
    [Inject] private PlaylistChanges Changes { get; set; } = default!;
    private bool _open;
    private bool _busy;
    private string? _selected;
    private string? _error;
    private string? _message;
    private PlaylistTrackReference? _track;
    private ManualPlaylist[]? _playlists;
    private int _version;
    private IEnumerable<SelectOption<string>> Options => _playlists?.Select(item =>
        new SelectOption<string>(item.Id.ToString(), $"{item.Name} ({item.Entries.Length})")) ?? [];

    public async Task ShowAsync(PlaylistTrackReference track)
    {
        _track = track;
        _open = true;
        _selected = null;
        _message = null;
        await LoadAsync();
        await InvokeAsync(StateHasChanged);
    }

    private void SetOpen(bool open)
    {
        _open = open;
        if (!open)
            _version++;
    }

    private async Task LoadAsync()
    {
        var version = ++_version;
        _busy = true;
        _error = null;
        var result = await Http.GetForResultAsync<ManualPlaylist[]>("api/playlists", Logger);
        if (version != _version)
            return;
        _busy = false;
        _playlists = result.IsSuccess ? result.Value : null;
        _error = result.IsFailed ? result.Errors[0].Message : null;
        if (_playlists is null || !_playlists.Any(item => item.Id.ToString() == _selected))
            _selected = null;
    }

    private async Task AddAsync()
    {
        if (_busy || _selected is null || _track is null)
            return;
        var version = ++_version;
        _busy = true;
        _error = _message = null;
        var result = await Http.PostForResultAsync<object, ManualPlaylist>(
            $"api/playlists/{_selected}/tracks", new { Track = _track }, Logger);
        if (result.IsSuccess)
            Changes.Notify();
        if (version != _version)
            return;
        _busy = false;
        if (result.IsFailed)
            _error = result.Errors[0].Message;
        else
        {
            _message = $"«{_track.FileName}» está en «{result.Value.Name}». Si ya existía, no se ha duplicado.";
            _playlists = _playlists?.Select(item => item.Id == result.Value.Id ? result.Value : item).ToArray();
        }
    }
}
