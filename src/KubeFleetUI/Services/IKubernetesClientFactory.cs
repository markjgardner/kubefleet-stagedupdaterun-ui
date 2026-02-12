using k8s;

namespace KubeFleetUI.Services;

public interface IKubernetesClientFactory
{
    IKubernetes CreateClient();
}
