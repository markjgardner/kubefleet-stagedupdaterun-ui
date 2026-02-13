# KubeFleet StagedUpdateRun UI

A Blazor Server web application for managing staged application rollouts across Kubernetes clusters using [KubeFleet](https://github.com/kubefleet-dev/kubefleet).

## Features

- **Dashboard** — View all StagedUpdateRun resources with status indicators
- **Flowchart Visualization** — Interactive pipeline view showing stages, clusters, and task status
- **Run Management** — Create, start, stop, resume, and delete staged update runs
- **Approval Workflow** — Approve pending stage tasks directly from the UI
- **Strategy Browser** — View configured StagedUpdateStrategy resources
- **Real-time Updates** — Watch API integration for live status changes

## Architecture

- **Framework**: .NET 10 / Blazor Server with Interactive Server rendering
- **UI Library**: Microsoft Fluent UI Blazor Components
- **Kubernetes Client**: KubernetesClient (C#) for custom resource operations
- **Authentication**: Azure Workload Identity via DefaultAzureCredential

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Access to a Kubernetes cluster with KubeFleet CRDs installed

### Local Development

```bash
# Clone the repository
git clone https://github.com/markjgardner/kubefleet-stagedupdaterun-ui.git
cd kubefleet-stagedupdaterun-ui

# Restore and build
dotnet restore
dotnet build

# Run tests
dotnet test

# Run the application
cd src/KubeFleetUI
dotnet run
```

### Configuration

Configure the Kubernetes API server URL in `appsettings.json` or `appsettings.Development.json`:

```json
{
  "Kubernetes": {
    "ApiServerUrl": "https://<hub-cluster-api>:443",
    "CertificateAuthorityData": "<Encoded CA>",
    "DefaultNamespace": "fleet-default"
  }
}
```

### Docker

```bash
docker build -t kubefleet-ui -f src/KubeFleetUI/Dockerfile .
docker run -p 8080:8080 kubefleet-ui
```

## Project Structure

```
src/
├── KubeFleetUI/              # Blazor Server application
│   ├── Components/           # Razor components (pages, layout, shared)
│   ├── Models/               # C# POCOs for KubeFleet CRDs
│   └── Services/             # Kubernetes service layer
└── KubeFleetUI.Tests/        # xUnit test project
```

## License

[MIT](LICENSE)
