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

public class ClusterPlacementServiceTests
{
    private readonly Mock<IKubernetesClientFactory> _factoryMock;
    private readonly Mock<IKubernetes> _clientMock;
    private readonly Mock<ICustomObjectsOperations> _customObjectsMock;
    private readonly ClusterPlacementService _service;

    public ClusterPlacementServiceTests()
    {
        _factoryMock = new Mock<IKubernetesClientFactory>();
        _clientMock = new Mock<IKubernetes>();
        _customObjectsMock = new Mock<ICustomObjectsOperations>();

        _clientMock.Setup(c => c.CustomObjects).Returns(_customObjectsMock.Object);
        _factoryMock.Setup(f => f.CreateClient()).Returns(_clientMock.Object);

        var logger = new Mock<ILogger<ClusterPlacementService>>();
        _service = new ClusterPlacementService(_factoryMock.Object, logger.Object);
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
    public async Task ListAsync_ReturnsClusterPlacements()
    {
        var placements = new List<ClusterResourcePlacement>
        {
            new() { Metadata = new ResourceMetadata { Name = "crp-1" } },
            new() { Metadata = new ResourceMetadata { Name = "crp-2" } }
        };
        var kubeList = new KubeList<ClusterResourcePlacement> { Items = placements };
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
        Assert.Equal("crp-1", result[0].Metadata.Name);
        Assert.Equal("crp-2", result[1].Metadata.Name);
    }

    [Fact]
    public async Task ListAsync_UsesCorrectApiGroupVersionAndPlural()
    {
        var kubeList = new KubeList<ClusterResourcePlacement> { Items = new List<ClusterResourcePlacement>() };
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
            KubeFleetConstants.PlacementVersion,
            KubeFleetConstants.ClusterResourcePlacementPlural,
            It.IsAny<bool?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>(),
            It.IsAny<bool?>(), It.IsAny<bool?>(),
            It.IsAny<IReadOnlyDictionary<string, IReadOnlyList<string>>>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
