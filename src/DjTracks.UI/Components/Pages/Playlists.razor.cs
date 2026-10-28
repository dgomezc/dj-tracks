using DjTracks.Domain;
using DjTracks.UI.Shared;
using FluentResults;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace DjTracks.UI.Components.Pages;

public partial class Playlists
{
    [Parameter] public Guid? Id { get; set; }
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private ILogger<Playlists> Logger { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private AudioPlayerState Player { get; set; } = default!;
    [Inject] private PlaylistChanges Changes { get; set; } = default!;
    private ManualPlaylist[]? _catalog;
    private ManualPlaylist? _playlist;
    private string? _error;
    private string? _message;
    private bool _busy;
    private bool _confirmDelete;
    private int _version;
    private bool _dialogOpen;
    private bool _renaming;
    private bool _dialogBusy;
    private string? _dialogName;
    private string? _dialogError;

    protected override Task OnParametersSetAsync() => LoadAsync();

    private async Task LoadAsync()
    {
        var version = ++_version;
        _busy = true;
        _error = _message = null;
        _playlist = null;
        _confirmDelete = false;
        var result = await Http.GetForResultAsync<ManualPlaylist[]>("api/playlists", Logger);
        if (version != _version)
            return;
        _catalog = result.IsSuccess ? result.Value : null;
        if (result.IsFailed)
            _error = result.Errors[0].Message;
        else if (Id is not null)
        {
            var detail = await Http.GetForResultAsync<ManualPlaylist>($"api/playlists/{Id}", Logger);
            if (version != _version)
                return;
            if (detail.IsFailed)
                _error = detail.Errors[0].Message;
            else
                _playlist = detail.Value;
        }
        _busy = false;
    }

    private void OpenCreate() => OpenDialog(renaming: false, name: null);

    private void OpenRename() => OpenDialog(renaming: true, name: _playlist?.Name);

    private void OpenDialog(bool renaming, string? name)
    {
        _renaming = renaming;
        _dialogName = name;
        _dialogError = null;
        _message = null;
        _dialogOpen = true;
    }

    private void OnDialogOpenChanged(bool open)
    {
        if (!open)
            CloseDialog();
    }

    private void CloseDialog()
    {
        if (_dialogBusy)
            return;
        _dialogOpen = false;
        _dialogName = _dialogError = null;
    }

    private async Task SaveDialogAsync()
    {
        if (_dialogBusy)
            return;
        _dialogBusy = true;
        _dialogError = null;
        var renaming = _renaming;
        var body = new { Name = _dialogName };
        var result = renaming
            ? await Http.PutForResultAsync<object, ManualPlaylist>($"api/playlists/{Id}", body, Logger)
            : await Http.PostForResultAsync<object, ManualPlaylist>("api/playlists", body, Logger);
        _dialogBusy = false;
        if (result.IsFailed)
        {
            _dialogError = result.Errors[0].Message;
            return;
        }
        Changes.Notify();
        CloseDialog();
        if (renaming)
        {
            Update(result.Value);
            _message = "Playlist renombrada.";
        }
        else
            Navigation.NavigateTo($"/playlists/{result.Value.Id}");
    }

    private Task DeleteAsync() => RunAsync(
        () => Http.DeleteForResultAsync<Guid>($"api/playlists/{Id}", Logger),
        _ => Navigation.NavigateTo("/playlists"), notify: true);

    private Task RemoveAsync(Guid entryId) => RunAsync(
        () => Http.DeleteForResultAsync<ManualPlaylist>($"api/playlists/{Id}/tracks/{entryId}", Logger),
        playlist => { Update(playlist); _message = "Canción quitada de la playlist. La cola no cambia."; }, notify: true);

    private Task LoadQueueAsync() => RunAsync(
        () => Http.GetForResultAsync<PlaylistPlayback>($"api/playlists/{Id}/playback", Logger),
        playback =>
        {
            var before = Player.Queue.Count;
            foreach (var track in playback.Tracks)
            {
                var reference = track.Reference;
                var isRemember = reference.Library == "Remember";
                var query = isRemember
                    ? new Dictionary<string, string?> { ["category"] = reference.Category, ["fileName"] = reference.FileName }
                    : new Dictionary<string, string?> { ["year"] = reference.Year, ["style"] = reference.Style, ["fileName"] = reference.FileName };
                var endpoint = isRemember ? "api/streaming/remember" : "api/streaming/tracks";
                string Url(string path) => new Uri(Http.BaseAddress!, QueryHelpers.AddQueryString(path, query)).AbsoluteUri;
                Player.Enqueue(Url(endpoint), Url($"{endpoint}/waveform"), Url($"{endpoint}/artwork"),
                    track.Details, rememberCategory: reference.Category);
            }
            _message = $"{Player.Queue.Count - before} canciones añadidas a la cola; {playback.Tracks.Length - (Player.Queue.Count - before)} ya estaban. La reproducción no se ha interrumpido.";
        });

    private async Task RunAsync<T>(Func<Task<Result<T>>> request, Action<T> success, bool notify = false)
    {
        if (_busy)
            return;
        var version = ++_version;
        _busy = true;
        _error = _message = null;
        var result = await request();
        if (result.IsSuccess && notify)
            Changes.Notify();
        if (version != _version)
            return;
        _busy = false;
        if (result.IsFailed)
            _error = result.Errors[0].Message;
        else
            success(result.Value);
    }

    private void Update(ManualPlaylist playlist)
    {
        _playlist = playlist;
        _catalog = _catalog?.Select(item => item.Id == playlist.Id ? playlist : item)
            .OrderBy(item => item.Name, StringComparer.OrdinalIgnoreCase).ToArray();
    }
}
