# Test CRD Configuration

This directory contains documentation for setting up KubeFleet CRDs for integration testing.

## CRD Source

The integration tests require the following KubeFleet CRDs:
- `StagedUpdateRun` - Represents a staged rollout operation
- `ApprovalRequest` - Represents a request to approve a stage
- `StagedUpdateStrategy` - Defines rollout strategies

These CRDs are sourced directly from the official KubeFleet repository rather than being vendored locally to ensure we're always testing against the latest official definitions.

## Using These CRDs

To run integration tests locally, you need a Kubernetes cluster with these CRDs installed:

```bash
# Create a test cluster with Kind
kind create cluster --name kubefleet-test

# Install the CRDs from the official kubefleet repository
kubectl apply -f https://raw.githubusercontent.com/kubefleet-dev/kubefleet/main/config/crd/bases/placement.kubernetes-fleet.io_stagedupdateruns.yaml
kubectl apply -f https://raw.githubusercontent.com/kubefleet-dev/kubefleet/main/config/crd/bases/placement.kubernetes-fleet.io_approvalrequests.yaml
kubectl apply -f https://raw.githubusercontent.com/kubefleet-dev/kubefleet/main/config/crd/bases/placement.kubernetes-fleet.io_stagedupdatestrategies.yaml

# Wait for CRDs to be established
kubectl wait --for condition=established --timeout=60s crd/stagedupdateruns.placement.kubernetes-fleet.io
kubectl wait --for condition=established --timeout=60s crd/approvalrequests.placement.kubernetes-fleet.io
kubectl wait --for condition=established --timeout=60s crd/stagedupdatestrategies.placement.kubernetes-fleet.io

# Set KUBECONFIG for tests
export KUBECONFIG=~/.kube/config

# Run integration tests
dotnet test --filter Category=Integration
```

## Official Source

Official KubeFleet CRDs are maintained at:
https://github.com/kubefleet-dev/kubefleet/tree/main/config/crd/bases
