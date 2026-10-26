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
    private bool _filtersOpen;
    private TrackFilterOptions? _filterOptions;
    private IReadOnlyCollection<string> _filterYears = [];
    private IReadOnlyCollection<string> _filterGroups = [];
    private IReadOnlyCollection<string> _filterSubstyles = [];
    private TrackFilter? _appliedFilter;

    protected override Task OnInitializedAsync() => LoadAsync(null, null);

    private Task OpenFolderAsync(string folder) =>
        _year is null ? LoadAsync(folder, null) : LoadAsync(_year, folder);

    private Task LoadAsync(string? year, string? style)
    {
        if (_busy)
            return Task.CompletedTask;
        _appliedFilter = null;
        _year = year;
        _style = style;
        return ReloadAsync();
    }

    private Task ApplyFiltersAsync()
    {
        if (_busy)
            return Task.CompletedTask;
        _appliedFilter = new(_filterYears.ToArray(), _filterGroups.ToArray(),
            _filterSubstyles.ToArray());
        _year = null;
        _style = null;
        return ReloadAsync();
    }

    private Task ClearFiltersAsync()
    {
        if (_busy)
            return Task.CompletedTask;
        _filterYears = _filterGroups = _filterSubstyles = [];
        return LoadAsync(null, null);
    }

    private async Task OnFiltersExpandedAsync(bool expanded)
    {
        _filtersOpen = expanded;
        if (expanded && !_busy)
            await RequestAsync(async () =>
                _filterOptions = await ReadApiAsync<TrackFilterOptions>("api/library/tracks/filter-options"));
    }

    private Task ReloadAsync() => RequestAsync(async () =>
    {
        _listing = null;
        if (_filtersOpen)
            _filterOptions = await ReadApiAsync<TrackFilterOptions>("api/library/tracks/filter-options");
        var url = "api/library/tracks";
        List<string> query = [];
        if (_appliedFilter is not null)
        {
            url += "/search";
            foreach (var year in _appliedFilter.Years)
                AddQuery(query, "year", year);
            foreach (var group in _appliedFilter.Groups)
                AddQuery(query, "group", group);
            foreach (var substyle in _appliedFilter.Substyles)
                AddQuery(query, "substyle", substyle);
        }
        else
        {
            AddQuery(query, "year", _year);
            AddQuery(query, "style", _style);
        }
        if (query.Count > 0)
            url += "?" + string.Join("&", query);
        _listing = await ReadApiAsync<TrackListing>(url);
    });

    private async Task<T> ReadApiAsync<T>(string url)
    {
        using var client = HttpClientFactory.CreateClient("Api");
        using var response = await client.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            throw new HttpRequestException(problem?.Detail ?? problem?.Title
                ?? $"La API devolvió un error ({(int)response.StatusCode}).", null, response.StatusCode);
        }
        return await response.Content.ReadFromJsonAsync<T>()
            ?? throw new JsonException("Respuesta de biblioteca vacía.");
    }

    private async Task RequestAsync(Func<Task> action)
    {
        if (_busy)
            return;
        _busy = true;
        _error = null;
        try
        {
            await action();
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException
            or JsonException or NotSupportedException)
        {
            Logger.LogError(ex, "No se pudo cargar la biblioteca de Tracks");
            _error = ex is HttpRequestException { StatusCode: not null }
                ? ex.Message
                : ex is JsonException or NotSupportedException
                ? "La API devolvió una respuesta no válida. Revisa el servidor y vuelve a cargar."
                : "No se pudo contactar con la API o se agotó el tiempo de espera. Comprueba que está iniciada y vuelve a cargar.";
        }
        finally
        {
            _busy = false;
        }
    }

    private static void AddQuery(List<string> query, string name, string? value)
    {
        if (value is not null)
            query.Add($"{name}={Uri.EscapeDataString(value)}");
    }

    private string FilterSummary => _appliedFilter is null ? ""
        : $"{SelectionSummary(_appliedFilter.Years, "Todos los años")} / {SelectionSummary(_appliedFilter.Groups, "Todos los estilos")} / {SelectionSummary(_appliedFilter.Substyles, "Todos los subestilos")}";

    private static string SelectionSummary(string[] values, string emptyText) =>
        values.Length == 0 ? emptyText : string.Join(", ", values);

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
