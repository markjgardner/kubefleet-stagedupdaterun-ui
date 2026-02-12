using System.Net;
using System.Net.Http;
using System.Text.Json;
using k8s;
using k8s.Autorest;
using KubeFleetUI.Models;
using KubeFleetUI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace KubeFleetUI.Tests.Services;

public class ApprovalServiceTests
{
    private readonly Mock<IKubernetesClientFactory> _factoryMock;
    private readonly Mock<IKubernetes> _clientMock;
    private readonly Mock<ICustomObjectsOperations> _customObjectsMock;
    private readonly ApprovalService _service;

    public ApprovalServiceTests()
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

        var logger = new Mock<ILogger<ApprovalService>>();
        _service = new ApprovalService(_factoryMock.Object, config, logger.Object);
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
    public async Task ListForRunAsync_ReturnsApprovalRequests()
    {
        var approvals = new List<ApprovalRequest>
        {
            new()
            {
                Metadata = new ResourceMetadata { Name = "approval1" },
                Spec = new ApprovalRequestSpec { TargetUpdateRun = "run1", TargetStage = "stage1" }
            }
        };
        var kubeList = new KubeList<ApprovalRequest> { Items = approvals };
        var jsonElement = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(kubeList));

        _customObjectsMock.Setup(c => c.ListNamespacedCustomObjectWithHttpMessagesAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<bool?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>(),
            It.IsAny<bool?>(), It.IsAny<bool?>(),
            It.IsAny<IReadOnlyDictionary<string, IReadOnlyList<string>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(WrapResponse(jsonElement));

        var result = await _service.ListForRunAsync("run1");

        Assert.Single(result);
        Assert.Equal("approval1", result[0].Metadata.Name);
    }

    [Fact]
    public async Task ApproveAsync_PatchesStatus()
    {
        _customObjectsMock.Setup(c => c.PatchNamespacedCustomObjectStatusWithHttpMessagesAsync(
            It.IsAny<object>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool?>(),
            It.IsAny<IReadOnlyDictionary<string, IReadOnlyList<string>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(WrapResponse(new object()));

        await _service.ApproveAsync("approval1");

        _customObjectsMock.Verify(c => c.PatchNamespacedCustomObjectStatusWithHttpMessagesAsync(
            It.IsAny<object>(),
            KubeFleetConstants.Group, KubeFleetConstants.Version, "test-ns",
            KubeFleetConstants.ApprovalRequestPlural, "approval1",
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool?>(),
            It.IsAny<IReadOnlyDictionary<string, IReadOnlyList<string>>>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
