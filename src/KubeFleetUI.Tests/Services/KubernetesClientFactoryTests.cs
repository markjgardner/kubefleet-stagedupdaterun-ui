using k8s;
using KubeFleetUI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace KubeFleetUI.Tests.Services;

public class KubernetesClientFactoryTests
{
    private readonly Mock<ILogger<KubernetesClientFactory>> _loggerMock = new();

    [Fact]
    public void CreateClient_WithCertificateAuthorityData_Succeeds()
    {
        // A minimal self-signed DER certificate encoded as base64
        var certBase64 = GenerateSelfSignedCertBase64();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Kubernetes:ApiServerUrl"] = "https://test-server:6443",
                ["Kubernetes:CertificateAuthorityData"] = certBase64
            })
            .Build();

        var factory = new KubernetesClientFactory(config, _loggerMock.Object);

        // Should not throw when creating a client with cert authority data
        var client = factory.CreateClient();
        Assert.NotNull(client);
    }

    [Fact]
    public void CreateClient_WithoutCertificateAuthorityData_Succeeds()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Kubernetes:ApiServerUrl"] = "https://test-server:6443"
            })
            .Build();

        var factory = new KubernetesClientFactory(config, _loggerMock.Object);

        var client = factory.CreateClient();
        Assert.NotNull(client);
    }

    [Fact]
    public void CreateClient_WithEmptyCertificateAuthorityData_Succeeds()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Kubernetes:ApiServerUrl"] = "https://test-server:6443",
                ["Kubernetes:CertificateAuthorityData"] = ""
            })
            .Build();

        var factory = new KubernetesClientFactory(config, _loggerMock.Object);

        var client = factory.CreateClient();
        Assert.NotNull(client);
    }

    [Fact]
    public void CreateClient_WithInvalidCertificateAuthorityData_ThrowsDescriptiveError()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Kubernetes:ApiServerUrl"] = "https://test-server:6443",
                ["Kubernetes:CertificateAuthorityData"] = "bm90LWEtdmFsaWQtY2VydA=="
            })
            .Build();

        var factory = new KubernetesClientFactory(config, _loggerMock.Object);

        var ex = Assert.Throws<InvalidOperationException>(() => factory.CreateClient());
        Assert.Contains("Kubernetes:CertificateAuthorityData", ex.Message);
    }

    private static string GenerateSelfSignedCertBase64()
    {
        using var rsa = System.Security.Cryptography.RSA.Create(2048);
        var request = new System.Security.Cryptography.X509Certificates.CertificateRequest(
            "CN=TestCA", rsa, System.Security.Cryptography.HashAlgorithmName.SHA256,
            System.Security.Cryptography.RSASignaturePadding.Pkcs1);
        using var cert = request.CreateSelfSigned(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddDays(1));
        return Convert.ToBase64String(cert.Export(System.Security.Cryptography.X509Certificates.X509ContentType.Cert));
    }
}
