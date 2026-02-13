# Integration Testing Guide

This guide explains how to run integration tests that validate API group references against a real Kubernetes API server.

## Overview

Integration tests use the `[Trait("Category", "Integration")]` attribute to distinguish them from unit tests. These tests:

- Connect to a real Kubernetes API server
- Validate that API group references are correct (e.g., `placement.kubernetes-fleet.io`)
- Ensure requests to invalid API groups are rejected
- Create and clean up test resources in isolated namespaces

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [kubectl](https://kubernetes.io/docs/tasks/tools/)
- [Kind](https://kind.sigs.k8s.io/) (Kubernetes in Docker) - for local testing
- Access to a Kubernetes cluster with KubeFleet CRDs installed

## Running Integration Tests Locally

### Option 1: Using Kind (Recommended)

1. **Create a test cluster**:
   ```bash
   kind create cluster --name kubefleet-test
   ```

2. **Install KubeFleet CRDs**:
   ```bash
   # Install CRDs from the official kubefleet repository
   kubectl apply -f https://raw.githubusercontent.com/kubefleet-dev/kubefleet/main/config/crd/bases/placement.kubernetes-fleet.io_stagedupdateruns.yaml
   kubectl apply -f https://raw.githubusercontent.com/kubefleet-dev/kubefleet/main/config/crd/bases/placement.kubernetes-fleet.io_approvalrequests.yaml
   kubectl apply -f https://raw.githubusercontent.com/kubefleet-dev/kubefleet/main/config/crd/bases/placement.kubernetes-fleet.io_stagedupdatestrategies.yaml
   
   # Wait for CRDs to be established
   kubectl wait --for condition=established --timeout=60s \
     crd/stagedupdateruns.placement.kubernetes-fleet.io
   kubectl wait --for condition=established --timeout=60s \
     crd/approvalrequests.placement.kubernetes-fleet.io
   kubectl wait --for condition=established --timeout=60s \
     crd/stagedupdatestrategies.placement.kubernetes-fleet.io
   ```

3. **Verify CRDs are installed**:
   ```bash
   kubectl get crds | grep placement.kubernetes-fleet.io
   ```

4. **Set KUBECONFIG** (if not already set):
   ```bash
   export KUBECONFIG=~/.kube/config
   ```

5. **Run integration tests**:
   ```bash
   dotnet test --filter Category=Integration
   ```

6. **Clean up**:
   ```bash
   kind delete cluster --name kubefleet-test
   ```

### Option 2: Using an Existing Cluster

If you have access to a cluster with KubeFleet CRDs already installed:

1. **Set KUBECONFIG** to point to your cluster:
   ```bash
   export KUBECONFIG=/path/to/your/kubeconfig
   ```

2. **Verify CRDs exist**:
   ```bash
   kubectl get crds | grep placement.kubernetes-fleet.io
   ```

3. **Run integration tests**:
   ```bash
   dotnet test --filter Category=Integration
   ```

## Running All Tests (Unit + Integration)

```bash
dotnet test
```

## Running Only Unit Tests

```bash
dotnet test --filter Category!=Integration
```

## Test Structure

Integration tests are located in `src/KubeFleetUI.Tests/Integration/`:

- **KubernetesClusterFixture.cs** - Test fixture that manages cluster connections and creates isolated test namespaces
- **ApiGroupValidationTests.cs** - Tests that validate API group correctness

## What These Tests Catch

These integration tests help prevent bugs like Issue #9, where incorrect API group references (e.g., `placement.kubefleet.io` instead of `placement.kubernetes-fleet.io`) can slip through without proper validation.

Specific scenarios covered:
- ✅ Resources can be created with the correct API group
- ✅ Resources cannot be created with incorrect API groups
- ✅ List operations use the correct API group
- ✅ Constants match the expected API group values

## CI/CD Integration

Integration tests run automatically in GitHub Actions:
- On pull requests to `main`
- On pushes to `main`
- Can be triggered manually via workflow dispatch

See `.github/workflows/integration-tests.yml` for the CI configuration.

## Troubleshooting

### "KUBECONFIG environment variable must be set"

Make sure you've set the `KUBECONFIG` environment variable:
```bash
export KUBECONFIG=~/.kube/config
```

### "Failed to create test namespace"

Check that you have permissions to create namespaces in the cluster:
```bash
kubectl auth can-i create namespaces
```

### CRD not found errors

Ensure the CRDs are installed and established:
```bash
kubectl get crds | grep placement.kubernetes-fleet.io
kubectl describe crd stagedupdateruns.placement.kubernetes-fleet.io
```

### Tests fail with 404 NotFound

This might indicate:
- CRDs are not installed
- API group in the code doesn't match the CRD definition
- Cluster is not accessible

Verify:
```bash
kubectl cluster-info
kubectl get crds
```

## Development Notes

- Each test run creates a unique namespace with a random name to avoid conflicts
- Test namespaces are automatically cleaned up after tests complete
- Tests are safe to run in parallel as they use isolated namespaces
- The fixture handles both setup and teardown of test resources
