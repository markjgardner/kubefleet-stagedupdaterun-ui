using KubeFleetUI.Services;
using Microsoft.FluentUI.AspNetCore.Components;

var builder = WebApplication.CreateBuilder(args);

// Add Fluent UI
builder.Services.AddFluentUIComponents();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Kubernetes services
builder.Services.AddSingleton<IKubernetesClientFactory, KubernetesClientFactory>();
builder.Services.AddScoped<IUpdateRunService, UpdateRunService>();
builder.Services.AddScoped<IClusterUpdateRunService, ClusterUpdateRunService>();
builder.Services.AddScoped<IStrategyService, StrategyService>();
builder.Services.AddScoped<IClusterStrategyService, ClusterStrategyService>();
builder.Services.AddScoped<IPlacementService, PlacementService>();
builder.Services.AddScoped<IClusterPlacementService, ClusterPlacementService>();
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
