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
    private string? _newName;
    private string? _name;
    private string? _error;
    private string? _message;
    private bool _busy;
    private bool _confirmDelete;
    private int _version;

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
            {
                _playlist = detail.Value;
                _name = _playlist.Name;
            }
        }
        _busy = false;
    }

    private Task CreateAsync() => RunAsync(
        () => Http.PostForResultAsync<object, ManualPlaylist>("api/playlists", new { Name = _newName }, Logger),
        playlist => { _newName = null; Navigation.NavigateTo($"/playlists/{playlist.Id}"); }, notify: true);

    private Task RenameAsync() => RunAsync(
        () => Http.PutForResultAsync<object, ManualPlaylist>($"api/playlists/{Id}", new { Name = _name }, Logger),
        playlist => { Update(playlist); _message = "Playlist renombrada."; }, notify: true);

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
        _name = playlist.Name;
        _catalog = _catalog?.Select(item => item.Id == playlist.Id ? playlist : item)
            .OrderBy(item => item.Name, StringComparer.OrdinalIgnoreCase).ToArray();
    }
}
