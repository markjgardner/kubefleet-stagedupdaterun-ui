using System.Text.Json;
using k8s;
using KubeFleetUI.Models;
using KubeFleetUI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace KubeFleetUI.Tests.Services;

public class StrategyServiceTests
{
    private readonly Mock<IKubernetesClientFactory> _factoryMock;
    private readonly Mock<IKubernetes> _clientMock;
    private readonly Mock<ICustomObjectsOperations> _customObjectsMock;
    private readonly StrategyService _service;

    public StrategyServiceTests()
    {
        _factoryMock = new Mock<IKubernetesClientFactory>();
        _clientMock = new Mock<IKubernetes>();
        _customObjectsMock = new Mock<ICustomObjectsOperations>();

        _clientMock.Setup(c => c.CustomObjects).Returns(_customObjectsMock.Object);
        _factoryMock.Setup(f => f.CreateClient()).Returns(_clientMock.Object);

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Kubernetes:DefaultNamespace"] = "test-ns"
            })
            .Build();

        var logger = new Mock<ILogger<StrategyService>>();
        _service = new StrategyService(_factoryMock.Object, config, logger.Object);
    }

    [Fact]
    public async Task ListAsync_ReturnsStrategies()
    {
        var strategies = new List<StagedUpdateStrategy>
        {
            new()
            {
                Metadata = new ResourceMetadata { Name = "strategy1" },
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
        var kubeList = new KubeList<StagedUpdateStrategy> { Items = strategies };
        var jsonElement = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(kubeList));

        _customObjectsMock.Setup(c => c.ListNamespacedCustomObjectAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<bool?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool?>(),
            It.IsAny<bool?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(jsonElement);

        var result = await _service.ListAsync();

        Assert.Single(result);
        Assert.Equal("strategy1", result[0].Metadata.Name);
        Assert.Equal(2, result[0].Spec.Stages.Count);
    }

    [Fact]
    public async Task GetAsync_ReturnsStrategy()
    {
        var strategy = new StagedUpdateStrategy
        {
            Metadata = new ResourceMetadata { Name = "test-strategy" },
            Spec = new UpdateStrategySpec
            {
                Stages = new List<StageConfig> { new() { Name = "stage1" } }
            }
        };
        var jsonElement = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(strategy));

        _customObjectsMock.Setup(c => c.GetNamespacedCustomObjectAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(jsonElement);

        var result = await _service.GetAsync("test-strategy");

        Assert.Equal("test-strategy", result.Metadata.Name);
        Assert.Single(result.Spec.Stages);
    }
}
