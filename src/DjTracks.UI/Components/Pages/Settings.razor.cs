using System.Net;
using System.Text.Json;
using DjTracks.Domain;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using MudBlazor;

namespace DjTracks.UI.Components.Pages;

public partial class Settings
{
    [Inject] private IHttpClientFactory HttpClientFactory { get; set; } = default!;
    [Inject] private ILogger<Settings> Logger { get; set; } = default!;
    [Inject] private IDialogService DialogService { get; set; } = default!;

    private PathSettings _settings = new();
    private Dictionary<string, string[]> _errors = new(StringComparer.OrdinalIgnoreCase);
    private bool _busy;
    private bool _loaded;
    private bool _saved;
    private string? _error;
    private bool Disabled => _busy || !_loaded;

    protected override Task OnInitializedAsync() => LoadAsync();

    private Task LoadAsync() => SendAsync(save: false);

    private Task SaveAsync() => Disabled ? Task.CompletedTask : SendAsync(save: true);

    private async Task BrowseAsync(string field, string initialPath, Action<string> setPath)
    {
        if (Disabled)
            return;

        var parameters = new DialogParameters<FolderPicker> { { picker => picker.InitialPath, initialPath } };
        var dialog = await DialogService.ShowAsync<FolderPicker>("Seleccionar carpeta del servidor",
            parameters, new DialogOptions { FullWidth = true, MaxWidth = MaxWidth.Medium });
        var result = await dialog.Result;
        if (result is { Canceled: false, Data: string path })
        {
            setPath(path);
            _errors.Remove(field);
            _saved = false;
        }
    }

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
            using var client = HttpClientFactory.CreateClient("Api");
            using var response = save
                ? await client.PutAsJsonAsync("api/settings", _settings)
                : await client.GetAsync("api/settings");

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode >= HttpStatusCode.InternalServerError)
                    _loaded = false;
                var problem = await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
                if (problem is not null)
                {
                    _errors = new(problem.Errors, StringComparer.OrdinalIgnoreCase);
                    _error = problem.Detail ?? problem.Title;
                }
                _error ??= $"La API devolvió un error ({(int)response.StatusCode}).";
                Logger.LogWarning("La API de ajustes devolvió HTTP {StatusCode}", (int)response.StatusCode);
                return;
            }

            _settings = await response.Content.ReadFromJsonAsync<PathSettings>()
                ?? throw new JsonException("Respuesta de ajustes vacía.");
            _loaded = true;
            _saved = save;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException
            or NotSupportedException)
        {
            Logger.LogError(ex, "No se pudo {Operation} la configuración", save ? "guardar" : "cargar");
            _error = ex is JsonException or NotSupportedException
                ? "La API devolvió una respuesta no válida. Revisa el servidor y vuelve a cargar."
                : "No se pudo contactar con la API o se agotó el tiempo de espera. Comprueba que está iniciada y vuelve a cargar para confirmar los ajustes.";
            _loaded = false;
        }
        finally
        {
            _busy = false;
        }
    }

    private bool HasError(string field) => _errors.ContainsKey(field);

    private string? FieldError(string field) =>
        _errors.TryGetValue(field, out var errors) ? string.Join(" ", errors) : null;
}
