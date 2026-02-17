using System.Net;
using System.Net.Http;
using System.Text.Json;
using k8s;
using k8s.Autorest;
using KubeFleetUI.Models;
using KubeFleetUI.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace KubeFleetUI.Tests.Services;

public class ClusterUpdateRunServiceTests
{
    private readonly Mock<IKubernetesClientFactory> _factoryMock;
    private readonly Mock<IKubernetes> _clientMock;
    private readonly Mock<ICustomObjectsOperations> _customObjectsMock;
    private readonly ClusterUpdateRunService _service;

    public ClusterUpdateRunServiceTests()
    {
        _factoryMock = new Mock<IKubernetesClientFactory>();
        _clientMock = new Mock<IKubernetes>();
        _customObjectsMock = new Mock<ICustomObjectsOperations>();

        _clientMock.Setup(c => c.CustomObjects).Returns(_customObjectsMock.Object);
        _factoryMock.Setup(f => f.CreateClient()).Returns(_clientMock.Object);

        var logger = new Mock<ILogger<ClusterUpdateRunService>>();
        _service = new ClusterUpdateRunService(_factoryMock.Object, logger.Object);
    }

    private static HttpOperationResponse<object> WrapResponse(object body)
    {
        return new HttpOperationResponse<object>
        {
            Body = body,
            Response = new HttpResponseMessage(HttpStatusCode.OK)
        };
    }

    [Fact]
    public async Task ListAsync_ReturnsClusterUpdateRuns()
    {
        var runs = new List<ClusterStagedUpdateRun>
        {
            new() { Metadata = new ResourceMetadata { Name = "cluster-run1" } },
            new() { Metadata = new ResourceMetadata { Name = "cluster-run2" } }
        };
        var kubeList = new KubeList<ClusterStagedUpdateRun> { Items = runs };
        var jsonElement = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(kubeList));

        _customObjectsMock.Setup(c => c.ListClusterCustomObjectWithHttpMessagesAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<bool?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>(),
            It.IsAny<bool?>(), It.IsAny<bool?>(),
            It.IsAny<IReadOnlyDictionary<string, IReadOnlyList<string>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(WrapResponse(jsonElement));

        var result = await _service.ListAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("cluster-run1", result[0].Metadata.Name);
        Assert.Equal("cluster-run2", result[1].Metadata.Name);
    }

    [Fact]
    public async Task ListAsync_UsesCorrectApiGroupAndPlural()
    {
        var kubeList = new KubeList<ClusterStagedUpdateRun> { Items = new List<ClusterStagedUpdateRun>() };
        var jsonElement = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(kubeList));

        _customObjectsMock.Setup(c => c.ListClusterCustomObjectWithHttpMessagesAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<bool?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>(),
            It.IsAny<bool?>(), It.IsAny<bool?>(),
            It.IsAny<IReadOnlyDictionary<string, IReadOnlyList<string>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(WrapResponse(jsonElement));

        await _service.ListAsync();

        _customObjectsMock.Verify(c => c.ListClusterCustomObjectWithHttpMessagesAsync(
            KubeFleetConstants.Group,
            KubeFleetConstants.Version,
            KubeFleetConstants.ClusterStagedUpdateRunPlural,
            It.IsAny<bool?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>(),
            It.IsAny<bool?>(), It.IsAny<bool?>(),
            It.IsAny<IReadOnlyDictionary<string, IReadOnlyList<string>>>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAsync_ReturnsClusterUpdateRun()
    {
        var run = new ClusterStagedUpdateRun
        {
            Metadata = new ResourceMetadata { Name = "cluster-run1" },
            Spec = new UpdateRunSpec { PlacementName = "crp-1" }
        };
        var jsonElement = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(run));

        _customObjectsMock.Setup(c => c.GetClusterCustomObjectWithHttpMessagesAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<IReadOnlyDictionary<string, IReadOnlyList<string>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(WrapResponse(jsonElement));

        var result = await _service.GetAsync("cluster-run1");

        Assert.Equal("cluster-run1", result.Metadata.Name);
        Assert.Equal("crp-1", result.Spec.PlacementName);
    }

    [Fact]
    public async Task CreateAsync_CreatesAndReturnsClusterRun()
    {
        var run = new ClusterStagedUpdateRun
        {
            Metadata = new ResourceMetadata { Name = "cluster-run1" },
            Spec = new UpdateRunSpec
            {
                PlacementName = "crp-1",
                StagedUpdateStrategyName = "cluster-strategy1"
            }
        };
        var jsonElement = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(run));

        _customObjectsMock.Setup(c => c.CreateClusterCustomObjectWithHttpMessagesAsync(
            It.IsAny<object>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<bool?>(),
            It.IsAny<IReadOnlyDictionary<string, IReadOnlyList<string>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(WrapResponse(jsonElement));

        var result = await _service.CreateAsync(run);

        Assert.Equal("cluster-run1", result.Metadata.Name);
        Assert.Equal("ClusterStagedUpdateRun", result.Kind);
    }

    [Fact]
    public async Task CreateAsync_UsesCorrectApiGroupAndPlural()
    {
        var run = new ClusterStagedUpdateRun
        {
            Metadata = new ResourceMetadata { Name = "cluster-run2" },
            Spec = new UpdateRunSpec
            {
                PlacementName = "crp-2",
                StagedUpdateStrategyName = "cluster-strategy2"
            }
        };
        var jsonElement = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(run));

        _customObjectsMock.Setup(c => c.CreateClusterCustomObjectWithHttpMessagesAsync(
            It.IsAny<object>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<bool?>(),
            It.IsAny<IReadOnlyDictionary<string, IReadOnlyList<string>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(WrapResponse(jsonElement));

        await _service.CreateAsync(run);

        _customObjectsMock.Verify(c => c.CreateClusterCustomObjectWithHttpMessagesAsync(
            It.IsAny<object>(),
            KubeFleetConstants.Group,
            KubeFleetConstants.Version,
            KubeFleetConstants.ClusterStagedUpdateRunPlural,
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<bool?>(),
            It.IsAny<IReadOnlyDictionary<string, IReadOnlyList<string>>>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_DeletesClusterRun()
    {
        _customObjectsMock.Setup(c => c.DeleteClusterCustomObjectWithHttpMessagesAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<k8s.Models.V1DeleteOptions>(),
            It.IsAny<int?>(), It.IsAny<bool?>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<IReadOnlyDictionary<string, IReadOnlyList<string>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(WrapResponse(new object()));

        await _service.DeleteAsync("cluster-run1");

        _customObjectsMock.Verify(c => c.DeleteClusterCustomObjectWithHttpMessagesAsync(
            KubeFleetConstants.Group, KubeFleetConstants.Version,
            KubeFleetConstants.ClusterStagedUpdateRunPlural, "cluster-run1",
            It.IsAny<k8s.Models.V1DeleteOptions>(),
            It.IsAny<int?>(), It.IsAny<bool?>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<IReadOnlyDictionary<string, IReadOnlyList<string>>>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateStateAsync_PatchesAndReturnsClusterRun()
    {
        var run = new ClusterStagedUpdateRun
        {
            Metadata = new ResourceMetadata { Name = "cluster-run1" },
            Spec = new UpdateRunSpec { State = UpdateRunState.Run }
        };
        var jsonElement = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(run));

        _customObjectsMock.Setup(c => c.PatchClusterCustomObjectWithHttpMessagesAsync(
            It.IsAny<object>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<bool?>(),
            It.IsAny<IReadOnlyDictionary<string, IReadOnlyList<string>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(WrapResponse(jsonElement));

        var result = await _service.UpdateStateAsync("cluster-run1", UpdateRunState.Run);

        Assert.Equal("cluster-run1", result.Metadata.Name);
        _customObjectsMock.Verify(c => c.PatchClusterCustomObjectWithHttpMessagesAsync(
            It.IsAny<object>(),
            KubeFleetConstants.Group,
            KubeFleetConstants.Version,
            KubeFleetConstants.ClusterStagedUpdateRunPlural,
            "cluster-run1",
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<bool?>(),
            It.IsAny<IReadOnlyDictionary<string, IReadOnlyList<string>>>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
