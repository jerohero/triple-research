using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Ardalis.Result;
using k8s;
using k8s.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json.Serialization;
using RealtimeCv.Core.Interfaces;
using RealtimeCv.Functions.Interfaces;
using RealtimeCv.Functions.Models;

namespace RealtimeCv.Functions.Controllers;

/// <summary>
/// Controller for Project related endpoints.
/// </summary>
public class ProjectController : BaseController
{
    private readonly ILoggerAdapter<ProjectController> _logger;
    private readonly IProjectService _projectService;
    private readonly ISessionService _sessionService;
    private readonly Kubernetes _kubernetes;

    public ProjectController(
      ILoggerAdapter<ProjectController> logger,
      IProjectService projectService,
      ISessionService sessionService,
      Kubernetes kubernetes
    )
    {
        _logger = logger;
        _projectService = projectService;
        _sessionService = sessionService;
        _kubernetes = kubernetes;
    }

    [Function("getProject")]
    public async Task<HttpResponseData> GetProject(
      [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "project/{projectId}")] HttpRequestData req, int projectId)
    {
        var result = await _projectService.GetProjectById(projectId);

        return await ResultToResponse(result, req);
    }

    [Function("getProjects")]
    public async Task<HttpResponseData> GetProjects(
      [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "project")] HttpRequestData req)
    {
        var result = await _projectService.GetProjects();

        return await ResultToResponse(result, req);
    }

    [Function("createProject")]
    public async Task<HttpResponseData> CreateProject(
      [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "project")] HttpRequestData req)
    {
        var projectCreateDto = DeserializeJson<ProjectCreateDto>(req.Body);

        var result = await _projectService.CreateProject(projectCreateDto);

        return await ResultToResponse(result, req);
    }

    [Function("updateProject")]
    public async Task<HttpResponseData> UpdateProject(
      [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "project")] HttpRequestData req)
    {
        var projectDto = DeserializeJson<ProjectUpdateDto>(req.Body);

        var result = await _projectService.UpdateProject(projectDto);

        return await ResultToResponse(result, req);
    }

    [Function("deleteProject")]
    public async Task<HttpResponseData> DeleteProject(
      [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "project/{projectId}")] HttpRequestData req, int projectId)
    {
        Result<ProjectDto> result = await _projectService.DeleteProject(projectId);

        return await ResultToResponse(result, req);
    }
    
    [Function("test")]
    public async Task<HttpResponseData> Test(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "test")] HttpRequestData req)
    {
        var deployment = await _kubernetes.ReadNamespacedDeploymentAsync("cv-deployment", "default");
        deployment.Spec.Replicas++;
        var updatedDeployment =
            await _kubernetes.ReplaceNamespacedDeploymentAsync(deployment, "cv-deployment", "default");
        
        // updatedDeployment.Status.

        return await ResultToResponse(new Result<string>("Fakka"), req);
    }
    
    [Function("test2")]
    public async Task<HttpResponseData> Test2(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "test2")] HttpRequestData req)
    {
        // var deployment = await _kubernetes.ReadNamespacedDeploymentAsync("cv-deployment", "default");
        // deployment.Spec.Replicas--;
        // var updatedDeployment =
        //     await _kubernetes.ReplaceNamespacedDeploymentAsync(deployment, "cv-deployment", "default");

        var createSessionDto = new SessionCreateDto(1, "rtmp://live.restream.io/live/re_6435068_ac960121c66cd1e6a9f5");
        
        var session = await _sessionService.CreateSession(createSessionDto);

        var podName = $"cv-pod-{session.Value.Id}";

        // TODO: Get vision set information from database

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
                        ImagePullPolicy = "IfNotPresent"
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
                        ImagePullPolicy = "IfNotPresent",
                        Env = new List<V1EnvVar>
                        {
                            new()
                            {
                                Name = "SESSION_ID",
                                Value = "1"
                            },
                            new()
                            {
                                Name = "STREAM_SOURCE",
                                Value = session.Value.Source
                            },
                        }
                    }
                }
            }
        };

        var createdPod = await _kubernetes.CreateNamespacedPodAsync(pod, "default");

        return await ResultToResponse(new Result<string>("Fakka"), req);
    }
}
