# Test CRD Manifests

This directory contains sample CustomResourceDefinition (CRD) manifests used for integration testing.

These CRDs define the KubeFleet resources that the UI interacts with:
- `StagedUpdateRun` - Represents a staged rollout operation
- `ApprovalRequest` - Represents a request to approve a stage
- `StagedUpdateStrategy` - Defines rollout strategies
- `ClusterStagedUpdateStrategy` - Cluster-scoped rollout strategies

## Using These CRDs

To run integration tests locally, you need a Kubernetes cluster with these CRDs installed:

```bash
# Create a test cluster with Kind
kind create cluster --name kubefleet-test

# Install the CRDs (use real KubeFleet CRDs if available)
kubectl apply -f test/crds/

# Set KUBECONFIG for tests
export KUBECONFIG=~/.kube/config

# Run integration tests
dotnet test --filter Category=Integration
```

## Note

These are simplified CRD definitions for testing purposes. In production, use the official KubeFleet CRDs from:
https://github.com/kubefleet-dev/kubefleet
