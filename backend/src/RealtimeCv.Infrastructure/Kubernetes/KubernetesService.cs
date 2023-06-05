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

        // new k8s.Kubernetes(new KubernetesClientConfiguration { Host = "http://localhost:8080/" }); // For local testing
        _kubernetes = new k8s.Kubernetes(kubeConfig);
    }
}
