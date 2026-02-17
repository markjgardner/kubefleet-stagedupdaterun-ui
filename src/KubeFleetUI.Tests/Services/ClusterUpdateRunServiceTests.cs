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
}
