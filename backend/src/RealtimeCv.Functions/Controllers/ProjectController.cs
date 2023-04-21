using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Ardalis.Result;
using k8s;
using k8s.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
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
    private readonly IVisionSetService _visionSetService;
    private readonly IKubernetesService _kubernetesService;

    public ProjectController(
        ILoggerAdapter<ProjectController> logger,
        IProjectService projectService,
        ISessionService sessionService,
        IVisionSetService visionSetService,
        IKubernetesService kubernetesService
    )
    {
        _logger = logger;
        _projectService = projectService;
        _sessionService = sessionService;
        _visionSetService = visionSetService;
        _kubernetesService = kubernetesService;
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

    [Function("test2")]
    public async Task<HttpResponseData> Test2(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "test2")] HttpRequestData req)
    {
        var createSessionDto = new SessionCreateDto(1, "rtmp://live.restream.io/live/re_6435068_ac960121c66cd1e6a9f5");

        var result = await _sessionService.CreateSession(createSessionDto);
        
        if (result.Errors.Any())
        {
            return await ResultToResponse(result, req);
        }

        await _kubernetesService.CreateCvPod(result.Value.Id, result.Value.Pod);

        return await ResultToResponse(result, req);
    }
}
