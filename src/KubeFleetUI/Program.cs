using KubeFleetUI.Dapr;
using KubeFleetUI.Services;
using Microsoft.FluentUI.AspNetCore.Components;

var builder = WebApplication.CreateBuilder(args);

// Add Fluent UI
builder.Services.AddFluentUIComponents();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Dapr (optional — only register if Dapr sidecar is available)
if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DAPR_HTTP_PORT")))
{
    builder.Services.AddSingleton(_ => new Dapr.Client.DaprClientBuilder().Build());
    builder.Services.AddSingleton<IDaprStateStore, DaprStateStore>();
    builder.Services.AddSingleton<IDaprSecretStore, DaprSecretStore>();
}

// Kubernetes services
builder.Services.AddSingleton<IKubernetesClientFactory, KubernetesClientFactory>();
builder.Services.AddScoped<IUpdateRunService, UpdateRunService>();
builder.Services.AddScoped<IStrategyService, StrategyService>();
builder.Services.AddScoped<IApprovalService, ApprovalService>();

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
