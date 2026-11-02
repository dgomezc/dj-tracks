using DjTracks.Api.Features.Library;
using DjTracks.Api.Features.Playlists;
using DjTracks.Api.Features.Settings;
using DjTracks.Api.Features.Streaming;
using DjTracks.Api.Shared;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
// Singleton: el filtro de validación los resuelve una vez al construir cada endpoint.
builder.Services.AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Singleton);
builder.Services.AddSettingsFeature();
builder.Services.AddLibraryFeature();
builder.Services.AddStreamingFeature();
builder.Services.AddPlaylistsFeature();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();

var api = app.MapGroup("/api").AddEndpointFilterFactory(ValidationFilter.Create);
api.MapSettingsFeature();
api.MapLibraryFeature();
api.MapStreamingFeature();
api.MapPlaylistsFeature();

app.Run();
