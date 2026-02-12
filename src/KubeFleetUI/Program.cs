using KubeFleetUI.Dapr;

var builder = WebApplication.CreateBuilder(args);

// Add Fluent UI
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Dapr (optional — only register if Dapr sidecar is available)
if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DAPR_HTTP_PORT")))
{
    builder.Services.AddDaprClient();
    builder.Services.AddSingleton<IDaprStateStore, DaprStateStore>();
    builder.Services.AddSingleton<IDaprSecretStore, DaprSecretStore>();
}

// Services will be registered here after Agent 2 creates them

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<KubeFleetUI.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
