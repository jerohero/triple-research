using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using k8s;
using k8s.KubeConfigModels;
using k8s.Models;
using RealtimeCv.Core.Entities;
using RealtimeCv.Core.Interfaces;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace RealtimeCv.Infrastructure.Kubernetes;

public class KubernetesService : IKubernetesService
{
    private readonly ILoggerAdapter<KubernetesService> _logger;
    private k8s.Kubernetes? _kubernetes;

    public KubernetesService(
        ILoggerAdapter<KubernetesService> logger
    )
    {
        _logger = logger;
    }

    public async Task<V1Pod> CreateSessionPod(Session session)
    {
        if (_kubernetes is null)
        {
            await InitKubernetes();
        }

        var pod = new V1Pod
        {
            Metadata = new V1ObjectMeta
            {
                Name = session.Pod,
                Labels = new Dictionary<string, string>
                {
                    { "app", session.VisionSet.Name }
                }
            },
            Spec = new V1PodSpec
            {
                Volumes = new List<V1Volume>
                {
                    new()
                    {
                        Name = "models-volume",
                        PersistentVolumeClaim = new V1PersistentVolumeClaimVolumeSource
                        {
                            ClaimName = "pvc-blobnfs-models"
                        }
                    }
                },
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
                        ImagePullPolicy = "Always",
                        VolumeMounts = new List<V1VolumeMount>
                        {
                            new ()
                            {
                                Name = "models-volume",
                                MountPath = "/mnt/models"
                            }
                        },
                        Resources = new V1ResourceRequirements
                        {
                            Requests = new Dictionary<string, ResourceQuantity>
                            {
                                { "memory", new ResourceQuantity("4Gi") },
                            },
                            Limits = new Dictionary<string, ResourceQuantity>
                            {
                                { "memory", new ResourceQuantity("5Gi") }
                            },
                        }
                    },
                    new()
                    {
                        Name = "cv-worker",
                        Image = "yeruhero/cv-worker:latest",
                        Ports = new List<V1ContainerPort>
                        {
                            new()
                            {
                                ContainerPort = 9300
                            }
                        },
                        ImagePullPolicy = "Always",
                        Resources = new V1ResourceRequirements
                        {
                            Requests = new Dictionary<string, ResourceQuantity>
                            {
                                { "memory", new ResourceQuantity("1Gi") },
                            },
                            Limits = new Dictionary<string, ResourceQuantity>
                            {
                                { "memory", new ResourceQuantity("1Gi") }
                            }
                        },
                        Env = new List<V1EnvVar>
                        {
                            new()
                            {
                                Name = "SessionId",
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
                            new()
                            {
                                Name = "AzureWebJobsStorage",
                                Value = Environment.GetEnvironmentVariable("AzureWebJobsStorage")
                            },
                            new()
                            {
                                Name = "BlobConnectionString",
                                Value = Environment.GetEnvironmentVariable("BlobConnectionString")
                            },
                            new()
                            {
                                Name = "SqlConnectionString",
                                Value = Environment.GetEnvironmentVariable("SqlConnectionString")
                            },
                            new()
                            {
                                Name = "AzureWebPubSub",
                                Value = Environment.GetEnvironmentVariable("AzureWebPubSub")
                            },
                            new()
                            {
                                Name = "ApiUrl",
                                Value = Environment.GetEnvironmentVariable("ApiUrl")
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
        if (_kubernetes is null)
        {
            await InitKubernetes();
        }

        return await _kubernetes.DeleteNamespacedPodAsync(podName, "default");
    }

    public async Task<V1Pod?> GetSessionPod(string podName)
    {
        if (_kubernetes is null)
        {
            await InitKubernetes();
        }

        V1Pod? pod;

        try
        {
            pod = await _kubernetes.ReadNamespacedPodAsync(podName, "default");
        }
        catch
        {
            pod = null;
        }
        
        return pod;
    }
    
    public async Task<V1PodList> GetVisionSetPods(string projectName, string visionSetName)
    {
        if (_kubernetes is null)
        {
            await InitKubernetes();
        }

        var pods = await _kubernetes.ListNamespacedPodAsync(
            "default",
            labelSelector: $"app={visionSetName}"
        );

        return pods;
    }
    
    private async Task InitKubernetes()
    {
        var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        var shareName = "config";
        var fileName = "kubeconfig";

        var shareClient = new ShareClient(connectionString, shareName);

        var fileClient = shareClient.GetRootDirectoryClient().GetFileClient(fileName);

        Response<ShareFileDownloadInfo> fileDownloadInfo = await fileClient.DownloadAsync();

        using var reader = new StreamReader(fileDownloadInfo.Value.Content);
        var kubeConfigYaml = await reader.ReadToEndAsync();

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        var kubeConfigObject = deserializer.Deserialize<K8SConfiguration>(kubeConfigYaml);

        var kubeConfig = KubernetesClientConfiguration.BuildConfigFromConfigObject(kubeConfigObject);

        // _kubernetes = new k8s.Kubernetes(new KubernetesClientConfiguration { Host = "http://localhost:8080/" }); // For local testing
        _kubernetes = new k8s.Kubernetes(kubeConfig);
    }
}
