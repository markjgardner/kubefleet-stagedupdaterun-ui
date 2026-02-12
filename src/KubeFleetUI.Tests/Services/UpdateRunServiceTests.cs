using System.Text.Json;
using k8s;
using KubeFleetUI.Models;
using KubeFleetUI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace KubeFleetUI.Tests.Services;

public class UpdateRunServiceTests
{
    private readonly Mock<IKubernetesClientFactory> _factoryMock;
    private readonly Mock<IKubernetes> _clientMock;
    private readonly Mock<ICustomObjectsOperations> _customObjectsMock;
    private readonly UpdateRunService _service;

    public UpdateRunServiceTests()
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

        var logger = new Mock<ILogger<UpdateRunService>>();
        _service = new UpdateRunService(_factoryMock.Object, config, logger.Object);
    }

    [Fact]
    public async Task ListAsync_ReturnsUpdateRuns()
    {
        var runs = new List<StagedUpdateRun>
        {
            new() { Metadata = new ResourceMetadata { Name = "run1" } },
            new() { Metadata = new ResourceMetadata { Name = "run2" } }
        };
        var kubeList = new KubeList<StagedUpdateRun> { Items = runs };
        var jsonElement = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(kubeList));

        _customObjectsMock.Setup(c => c.ListNamespacedCustomObjectAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<bool?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool?>(),
            It.IsAny<bool?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(jsonElement);

        var result = await _service.ListAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("run1", result[0].Metadata.Name);
        Assert.Equal("run2", result[1].Metadata.Name);
    }

    [Fact]
    public async Task GetAsync_ReturnsUpdateRun()
    {
        var run = new StagedUpdateRun
        {
            Metadata = new ResourceMetadata { Name = "test-run", Namespace = "test-ns" },
            Spec = new UpdateRunSpec { PlacementName = "placement1" }
        };
        var jsonElement = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(run));

        _customObjectsMock.Setup(c => c.GetNamespacedCustomObjectAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(jsonElement);

        var result = await _service.GetAsync("test-run");

        Assert.Equal("test-run", result.Metadata.Name);
        Assert.Equal("placement1", result.Spec.PlacementName);
    }

    [Fact]
    public async Task CreateAsync_CreatesAndReturnsRun()
    {
        var run = new StagedUpdateRun
        {
            Metadata = new ResourceMetadata { Name = "new-run" },
            Spec = new UpdateRunSpec
            {
                PlacementName = "placement1",
                StagedUpdateStrategyName = "strategy1"
            }
        };
        var jsonElement = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(run));

        _customObjectsMock.Setup(c => c.CreateNamespacedCustomObjectAsync(
            It.IsAny<object>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(jsonElement);

        var result = await _service.CreateAsync(run);

        Assert.Equal("new-run", result.Metadata.Name);
    }

    [Fact]
    public async Task DeleteAsync_DeletesRun()
    {
        _customObjectsMock.Setup(c => c.DeleteNamespacedCustomObjectAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<int?>(), It.IsAny<bool?>(), It.IsAny<string>(), It.IsAny<k8s.Models.V1DeleteOptions>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new object());

        await _service.DeleteAsync("test-run");

        _customObjectsMock.Verify(c => c.DeleteNamespacedCustomObjectAsync(
            KubeFleetConstants.Group, KubeFleetConstants.Version, "test-ns",
            KubeFleetConstants.StagedUpdateRunPlural, "test-run",
            It.IsAny<int?>(), It.IsAny<bool?>(), It.IsAny<string>(), It.IsAny<k8s.Models.V1DeleteOptions>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ListAsync_UsesCustomNamespace()
    {
        var kubeList = new KubeList<StagedUpdateRun> { Items = new List<StagedUpdateRun>() };
        var jsonElement = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(kubeList));

        _customObjectsMock.Setup(c => c.ListNamespacedCustomObjectAsync(
            It.IsAny<string>(), It.IsAny<string>(), "custom-ns", It.IsAny<string>(),
            It.IsAny<bool?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool?>(),
            It.IsAny<bool?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(jsonElement);

        await _service.ListAsync("custom-ns");

        _customObjectsMock.Verify(c => c.ListNamespacedCustomObjectAsync(
            It.IsAny<string>(), It.IsAny<string>(), "custom-ns", It.IsAny<string>(),
            It.IsAny<bool?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool?>(),
            It.IsAny<bool?>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
