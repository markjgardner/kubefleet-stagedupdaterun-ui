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

public class ClusterStrategyServiceTests
{
    private readonly Mock<IKubernetesClientFactory> _factoryMock;
    private readonly Mock<IKubernetes> _clientMock;
    private readonly Mock<ICustomObjectsOperations> _customObjectsMock;
    private readonly ClusterStrategyService _service;

    public ClusterStrategyServiceTests()
    {
        _factoryMock = new Mock<IKubernetesClientFactory>();
        _clientMock = new Mock<IKubernetes>();
        _customObjectsMock = new Mock<ICustomObjectsOperations>();

        _clientMock.Setup(c => c.CustomObjects).Returns(_customObjectsMock.Object);
        _factoryMock.Setup(f => f.CreateClient()).Returns(_clientMock.Object);

        var logger = new Mock<ILogger<ClusterStrategyService>>();
        _service = new ClusterStrategyService(_factoryMock.Object, logger.Object);
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
    public async Task ListAsync_ReturnsClusterStrategies()
    {
        var strategies = new List<ClusterStagedUpdateStrategy>
        {
            new()
            {
                Metadata = new ResourceMetadata { Name = "cluster-strategy1" },
                Spec = new UpdateStrategySpec
                {
                    Stages = new List<StageConfig>
                    {
                        new() { Name = "stage1" },
                        new() { Name = "stage2" }
                    }
                }
            }
        };
        var kubeList = new KubeList<ClusterStagedUpdateStrategy> { Items = strategies };
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

        Assert.Single(result);
        Assert.Equal("cluster-strategy1", result[0].Metadata.Name);
        Assert.Equal(2, result[0].Spec.Stages.Count);
    }

    [Fact]
    public async Task GetAsync_ReturnsClusterStrategy()
    {
        var strategy = new ClusterStagedUpdateStrategy
        {
            Metadata = new ResourceMetadata { Name = "test-cluster-strategy" },
            Spec = new UpdateStrategySpec
            {
                Stages = new List<StageConfig> { new() { Name = "stage1" } }
            }
        };
        var jsonElement = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(strategy));

        _customObjectsMock.Setup(c => c.GetClusterCustomObjectWithHttpMessagesAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<IReadOnlyDictionary<string, IReadOnlyList<string>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(WrapResponse(jsonElement));

        var result = await _service.GetAsync("test-cluster-strategy");

        Assert.Equal("test-cluster-strategy", result.Metadata.Name);
        Assert.Single(result.Spec.Stages);
    }
}
