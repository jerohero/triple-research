using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using k8s;
using k8s.Models;
using Newtonsoft.Json;
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

    public async Task<V1Pod> CreateSessionPod(Session session)
    {
        var pod = new V1Pod
        {
            Metadata = new V1ObjectMeta
            {
                Name = session.Pod, Labels = new Dictionary<string, string>
                {
                    { "app", session.Pod }
                }
            },
            Spec = new V1PodSpec
            {
                Containers = new List<V1Container>
                {
                    new()
                    {
                        Name = "cv-inference",
                        Image = session.VisionSet.ContainerImage,
                        Ports = new List<V1ContainerPort>
                        {
                            new() {
                                ContainerPort = 5000
                            }
                        },
                        ImagePullPolicy = "Always" // TODO: Always
                    },
                    new()
                    {
                        Name = "cv-worker",
                        Image = "yeruhero/cv-worker:latest", // TODO Env
                        Ports = new List<V1ContainerPort>
                        {
                            new()
                            {
                                ContainerPort = 9300
                            }
                        },
                        ImagePullPolicy = "Always", // TODO IfNotPresent
                        Env = new List<V1EnvVar>
                        {
                            new()
                            {
                                Name = "SESSION_ID",
                                Value = session.Id.ToString()
                            },
                            new()
                            {
                                Name = "StorageAccountName",
                                Value = Environment.GetEnvironmentVariable("StorageAccountName")
                            },
                            new()
                            {
                                Name = "StorageAccountKey",
                                Value = Environment.GetEnvironmentVariable("StorageAccountKey")
                            },
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
