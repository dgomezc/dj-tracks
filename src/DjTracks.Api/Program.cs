using DjTracks.Api.Features.Library;
using DjTracks.Api.Features.Playlists;
using DjTracks.Api.Features.Streaming;
using DjTracks.Api.Features.Stats;
using DjTracks.Api.Features.Subsonic;
using DjTracks.Api.Shared;
using FluentValidation;
using DjTracks.Infrastructure.Settings;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddFilter("Microsoft.AspNetCore.Hosting.Diagnostics", LogLevel.Warning);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
// Singleton: el filtro de validación los resuelve una vez al construir cada endpoint.
builder.Services.AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Singleton);
builder.Services.AddOptions<PathSettings>()
    .BindConfiguration("Paths")
    .Validate(paths => !string.IsNullOrWhiteSpace(paths.Tracks) && Path.IsPathFullyQualified(paths.Tracks),
        "Paths:Tracks debe ser una ruta absoluta del servidor de la API.")
    .Validate(paths => !string.IsNullOrWhiteSpace(paths.Sessions) && Path.IsPathFullyQualified(paths.Sessions),
        "Paths:Sessions debe ser una ruta absoluta del servidor de la API.")
    .Validate(paths => !string.IsNullOrWhiteSpace(paths.Remember) && Path.IsPathFullyQualified(paths.Remember),
        "Paths:Remember debe ser una ruta absoluta del servidor de la API.")
    .Validate(paths => !string.IsNullOrWhiteSpace(paths.Data) && Path.IsPathFullyQualified(paths.Data),
        "Paths:Data debe ser una ruta absoluta del servidor de la API.")
    .ValidateOnStart();
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<PathSettings>>().Value);
builder.Services.AddLibraryFeature();
builder.Services.AddStreamingFeature();
builder.Services.AddPlaylistsFeature();
builder.Services.AddStatsFeature();
builder.Services.AddSubsonicFeature();

var app = builder.Build();
app.Services.GetRequiredService<PathSettings>().InitializeDataDirectories();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();

app.MapSubsonicFeature();
var api = app.MapGroup("/api").AddEndpointFilterFactory(ValidationFilter.Create);
api.MapLibraryFeature();
api.MapStreamingFeature();
api.MapPlaylistsFeature();
api.MapStatsFeature();

app.Run();
