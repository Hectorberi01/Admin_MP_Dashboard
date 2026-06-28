using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Admin_MP_Dashboard;
using Admin_MP_Dashboard.Api;
using Admin_MP_Dashboard.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Mock (mode démo, sans réseau).
builder.Services.AddSingleton<MockAdminApi>();

// Session admin (jeton) partagée à toute l'app.
builder.Services.AddSingleton<AuthState>();

// --- BFF Admin ---
// Bascule via wwwroot/appsettings.json : "Api": { "UseMock": ..., "BaseUrl": ... }
var useMock = bool.TryParse(builder.Configuration["Api:UseMock"], out var m) && m;
var baseUrl = builder.Configuration["Api:BaseUrl"];

if (useMock || string.IsNullOrWhiteSpace(baseUrl))
{
    builder.Services.AddSingleton<IAdminApi>(sp => sp.GetRequiredService<MockAdminApi>());
}
else
{
    builder.Services.AddTransient<BearerAuthHandler>();
    builder.Services
        .AddHttpClient<IAdminApi, HttpAdminApi>(client => client.BaseAddress = new Uri(baseUrl))
        .AddHttpMessageHandler<BearerAuthHandler>();
}

var host = builder.Build();

await host.Services.GetRequiredService<AuthState>().InitializeAsync();

await host.RunAsync();
