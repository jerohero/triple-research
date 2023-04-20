using System.Collections.Generic;
using System.Threading.Tasks;
using k8s;
using k8s.Models;
using RealtimeCv.Core.Entities;
using RealtimeCv.Core.Interfaces;

namespace RealtimeCv.Infrastructure.Kubernetes;

public class KubernetesService : IKubernetesService
{
    private k8s.Kubernetes _kubernetes;
    
    public KubernetesService(
        k8s.Kubernetes kubernetes
    )
    {
        _kubernetes = kubernetes;
    }

    public async Task<V1Pod> CreateCvPod(int sessionId, string visionSetName)
    {
        var podName = $"cv-{visionSetName}-{sessionId}";

        var pod = new V1Pod
        {
            Metadata = new V1ObjectMeta
            {
                Name = podName, Labels = new Dictionary<string, string>
                {
                    { "app", podName }
                }
            },
            Spec = new V1PodSpec
            {
                Containers = new List<V1Container>
                {
                    new()
                    {
                        Name = "cv-inference",
                        Image = "yeruhero/yolov3api:latest", // TODO VisionSet image
                        Ports = new List<V1ContainerPort>
                        {
                            new() {
                                ContainerPort = 5000
                            }
                        },
                        ImagePullPolicy = "Always" // TODO: IfNotPresent
                    },
                    new()
                    {
                        Name = "cv-worker",
                        Image = "yeruhero/cv-worker:latest", // TODO VisionSet image
                        Ports = new List<V1ContainerPort>
                        {
                            new()
                            {
                                ContainerPort = 9300
                            }
                        },
                        ImagePullPolicy = "Always", // TODO Always
                        Env = new List<V1EnvVar>
                        {
                            new()
                            {
                                Name = "SESSION_ID",
                                Value = sessionId.ToString()
                            }
                        }
                    }
                }
            }
        };
        
        return await _kubernetes.CreateNamespacedPodAsync(pod, "default");
    }

    public async Task<V1Pod> DeletePod(string podName)
    {
        return await _kubernetes.DeleteNamespacedPodAsync(podName, "default");
    }
}
