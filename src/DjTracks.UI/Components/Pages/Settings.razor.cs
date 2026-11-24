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

    private IJSObjectReference? _module;
    private string? _theme;
    private bool _testingStatistics;
    private StatisticsConnectionStatus? _statisticsStatus;
    private string? _statisticsError;

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

    private async Task TestStatisticsConnectionAsync()
    {
        if (_testingStatistics)
            return;

        _testingStatistics = true;
        _statisticsStatus = null;
        _statisticsError = null;
        try
        {
            var result = await Http.GetForResultAsync<StatisticsConnectionStatus>("api/stats/connection", Logger);
            if (result.IsFailed)
                _statisticsError = result.Errors[0].Message;
            else
                _statisticsStatus = result.Value;
        }
        finally
        {
            _testingStatistics = false;
        }
    }

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