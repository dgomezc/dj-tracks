using System.Text.Json;
using DjTracks.Domain;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;

namespace DjTracks.UI.Components.Pages;

public partial class Tracks
{
    [Inject] private IHttpClientFactory HttpClientFactory { get; set; } = default!;
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
            using var client = HttpClientFactory.CreateClient("Api");
            using var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
                _error = problem?.Detail ?? problem?.Title
                    ?? $"La API devolvió un error ({(int)response.StatusCode}).";
                Logger.LogWarning("La API de Tracks devolvió HTTP {StatusCode}", (int)response.StatusCode);
                return;
            }
            _listing = await response.Content.ReadFromJsonAsync<TrackListing>()
                ?? throw new JsonException("Respuesta de biblioteca vacía.");
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException
            or JsonException or NotSupportedException)
        {
            Logger.LogError(ex, "No se pudo cargar la biblioteca de Tracks");
            _error = ex is JsonException or NotSupportedException
                ? "La API devolvió una respuesta no válida. Revisa el servidor y vuelve a cargar."
                : "No se pudo contactar con la API o se agotó el tiempo de espera. Comprueba que está iniciada y vuelve a cargar.";
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
