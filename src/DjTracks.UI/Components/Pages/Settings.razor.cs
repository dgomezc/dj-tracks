using DjTracks.Domain;
using DjTracks.UI.Shared;
using BlazorBlueprint.Primitives;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DjTracks.UI.Components.Pages;

public partial class Settings : IAsyncDisposable
{
    private static readonly SelectOption<string>[] ThemeOptions =
        AppThemes.All.Select(theme => new SelectOption<string>(theme.Id, theme.Name)).ToArray();

    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private ILogger<Settings> Logger { get; set; } = default!;
    [Inject] private IJSRuntime JS { get; set; } = default!;

    private PathSettings _settings = new();
    private Dictionary<string, string[]> _errors = new(StringComparer.OrdinalIgnoreCase);
    private bool _busy;
    private bool _loaded;
    private bool _saved;
    private string? _error;
    private bool Disabled => _busy || !_loaded;
    private IJSObjectReference? _module;
    private string? _theme;

    protected override Task OnInitializedAsync() => LoadAsync();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        _module = await JS.InvokeAsync<IJSObjectReference>("import", "./Components/Pages/Settings.razor.js");
        _theme = AppThemes.Resolve(await _module.InvokeAsync<string?>("getTheme"));
        StateHasChanged();
    }

    private async Task ChangeThemeAsync(string? theme)
    {
        if (_module is null || theme is null)
            return;

        _theme = AppThemes.Resolve(theme);
        await _module.InvokeVoidAsync("setTheme", _theme);
    }

    private Task LoadAsync() => SendAsync(save: false);

    private Task SaveAsync() => Disabled ? Task.CompletedTask : SendAsync(save: true);

    private async Task SendAsync(bool save)
    {
        _busy = true;
        _saved = false;
        _error = null;
        _errors.Clear();
        if (!save)
            _loaded = false;

        try
        {
            var result = save
                ? await Http.PutForResultAsync<PathSettings, PathSettings>("api/settings", _settings, Logger)
                : await Http.GetForResultAsync<PathSettings>("api/settings", Logger);
            if (result.IsFailed)
            {
                var error = (AppError)result.Errors[0];
                if (error.FieldErrors.Count > 0)
                {
                    _errors = new(error.FieldErrors, StringComparer.OrdinalIgnoreCase);
                    _error = "Revisa las rutas indicadas.";
                }
                else
                {
                    // Sin errores por campo: ajustes o API inaccesibles; no se permite guardar.
                    _error = error.Message;
                    _loaded = false;
                }
                return;
            }

            _settings = result.Value;
            _loaded = true;
            _saved = save;
        }
        finally
        {
            _busy = false;
        }
    }

    private bool HasError(string field) => _errors.ContainsKey(field);

    private IEnumerable<string>? FieldErrors(string field) =>
        _errors.TryGetValue(field, out var errors) ? errors : null;

    public async ValueTask DisposeAsync()
    {
        if (_module is null)
            return;

        try
        {
            await _module.DisposeAsync();
        }
        catch (JSDisconnectedException)
        {
            Logger.LogDebug("El circuito se desconectó al liberar el módulo de Configuración");
        }
    }
}