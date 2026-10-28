using DjTracks.UI.Components;
using DjTracks.UI.Shared;
using BlazorBlueprint.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
// Radio de 1rem del tema "Light Green" de tweakcn; el ThemeService lo aplica en <html>.
builder.Services.AddBlazorBlueprintComponents(configureTheme: theme => theme.DefaultRadius = 1.0);
builder.Services.AddScoped<AudioPlayerState>();
builder.Services.AddHttpClient("Api", client =>
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]!));
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Api"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
