using System.Net;
using k8s;
using k8s.Autorest;
using KubeFleetUI.Models;

namespace KubeFleetUI.Tests.Integration;

/// <summary>
/// Integration tests that validate API group references against a real Kubernetes API server.
/// These tests catch errors like using 'placement.kubefleet.io' instead of 'placement.kubernetes-fleet.io'.
/// </summary>
[Collection("Kubernetes Integration Tests")]
public class ApiGroupValidationTests : IClassFixture<KubernetesClusterFixture>
{
    private readonly KubernetesClusterFixture _fixture;

    public ApiGroupValidationTests(KubernetesClusterFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task StagedUpdateRun_UsesCorrectApiGroup()
    {
        // Arrange
        var client = _fixture.Client;
        var testRun = new StagedUpdateRun
        {
            Metadata = new ResourceMetadata
            {
                Name = "test-run-" + Guid.NewGuid().ToString("N")[..8],
                Namespace = _fixture.TestNamespace
            },
            Spec = new UpdateRunSpec
            {
                StagedUpdateStrategyName = "test-strategy",
                PlacementName = "test-placement"
            }
        };

        // Act - Create the resource using the correct API group
        var exception = await Record.ExceptionAsync(async () =>
        {
            await client.CustomObjects.CreateNamespacedCustomObjectAsync(
                testRun,
                KubeFleetConstants.Group,
                KubeFleetConstants.Version,
                _fixture.TestNamespace,
                KubeFleetConstants.StagedUpdateRunPlural);
        });

        // Assert - Should succeed if CRDs are installed with correct API group
        Assert.Null(exception);

        // Cleanup
        if (exception == null)
        {
            await client.CustomObjects.DeleteNamespacedCustomObjectAsync(
                KubeFleetConstants.Group,
                KubeFleetConstants.Version,
                _fixture.TestNamespace,
                KubeFleetConstants.StagedUpdateRunPlural,
                testRun.Metadata.Name);
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task StagedUpdateRun_WithIncorrectApiGroup_IsRejected()
    {
        // Arrange
        var client = _fixture.Client;
        var incorrectGroup = "placement.kubefleet.io"; // This is the wrong API group
        var testRun = new StagedUpdateRun
        {
            Metadata = new ResourceMetadata
            {
                Name = "test-run-" + Guid.NewGuid().ToString("N")[..8],
                Namespace = _fixture.TestNamespace
            },
            Spec = new UpdateRunSpec
            {
                StagedUpdateStrategyName = "test-strategy",
                PlacementName = "test-placement"
            }
        };

        // Act & Assert - Should fail with 404 NotFound
        var exception = await Assert.ThrowsAsync<HttpOperationException>(async () =>
        {
            await client.CustomObjects.CreateNamespacedCustomObjectAsync(
                testRun,
                incorrectGroup, // Using incorrect API group
                KubeFleetConstants.Version,
                _fixture.TestNamespace,
                KubeFleetConstants.StagedUpdateRunPlural);
        });

        Assert.Equal(HttpStatusCode.NotFound, exception.Response.StatusCode);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task ApprovalRequest_UsesCorrectApiGroup()
    {
        // Arrange
        var client = _fixture.Client;
        var testApproval = new ApprovalRequest
        {
            Metadata = new ResourceMetadata
            {
                Name = "test-approval-" + Guid.NewGuid().ToString("N")[..8],
                Namespace = _fixture.TestNamespace
            },
            Spec = new ApprovalRequestSpec
            {
                TargetUpdateRun = "test-run",
                TargetStage = "stage-1"
            }
        };

        // Act - Create the resource using the correct API group
        var exception = await Record.ExceptionAsync(async () =>
        {
            await client.CustomObjects.CreateNamespacedCustomObjectAsync(
                testApproval,
                KubeFleetConstants.Group,
                KubeFleetConstants.Version,
                _fixture.TestNamespace,
                KubeFleetConstants.ApprovalRequestPlural);
        });

        // Assert - Should succeed if CRDs are installed with correct API group
        Assert.Null(exception);

        // Cleanup
        if (exception == null)
        {
            await client.CustomObjects.DeleteNamespacedCustomObjectAsync(
                KubeFleetConstants.Group,
                KubeFleetConstants.Version,
                _fixture.TestNamespace,
                KubeFleetConstants.ApprovalRequestPlural,
                testApproval.Metadata.Name);
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task ListStagedUpdateRuns_UsesCorrectApiGroup()
    {
        // Arrange
        var client = _fixture.Client;

        // Act - List resources using the correct API group
        var exception = await Record.ExceptionAsync(async () =>
        {
            await client.CustomObjects.ListNamespacedCustomObjectAsync(
                KubeFleetConstants.Group,
                KubeFleetConstants.Version,
                _fixture.TestNamespace,
                KubeFleetConstants.StagedUpdateRunPlural);
        });

        // Assert - Should succeed (may return empty list)
        Assert.Null(exception);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task ListStagedUpdateRuns_WithIncorrectApiGroup_IsRejected()
    {
        // Arrange
        var client = _fixture.Client;
        var incorrectGroup = "placement.kubefleet.io"; // This is the wrong API group

        // Act & Assert - Should fail with 404 NotFound
        var exception = await Assert.ThrowsAsync<HttpOperationException>(async () =>
        {
            await client.CustomObjects.ListNamespacedCustomObjectAsync(
                incorrectGroup, // Using incorrect API group
                KubeFleetConstants.Version,
                _fixture.TestNamespace,
                KubeFleetConstants.StagedUpdateRunPlural);
        });

        Assert.Equal(HttpStatusCode.NotFound, exception.Response.StatusCode);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task KubeFleetConstants_ContainsCorrectApiGroup()
    {
        // This test validates that our constants are correct
        Assert.Equal("placement.kubernetes-fleet.io", KubeFleetConstants.Group);
        Assert.NotEqual("placement.kubefleet.io", KubeFleetConstants.Group);
        
        // Verify it's the same as what's in the model - use exact match
        var testRun = new StagedUpdateRun();
        Assert.Equal("placement.kubernetes-fleet.io/v1beta1", testRun.ApiVersion);
    }
}
