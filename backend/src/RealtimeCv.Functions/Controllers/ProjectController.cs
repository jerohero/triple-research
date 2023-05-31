using System.Threading.Tasks;
using Ardalis.Result;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using RealtimeCv.Core.Functions.Interfaces;
using RealtimeCv.Core.Interfaces;
using RealtimeCv.Core.Models.Dto;

namespace RealtimeCv.Functions.Controllers;

/// <summary>
/// Controller for Project related endpoints.
/// </summary>
public class ProjectController : BaseController
{
    private readonly ILoggerAdapter<ProjectController> _logger;
    private readonly IProjectService _projectService;

    public ProjectController(
        ILoggerAdapter<ProjectController> logger,
        IProjectService projectService
    )
    {
        _logger = logger;
        _projectService = projectService;
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
        var result = await _projectService.DeleteProject(projectId);

        return await ResultToResponse(result, req);
    }

    [Function("uploadTrainedModelChunk")]
    public async Task<HttpResponseData> UploadTrainedModelChunk(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "project/{projectId}/trained-model")] HttpRequestData req, int projectId)
    {
        var stream = req.Body;
        var metadata = ExtractMetadata<TrainedModelChunkMetadata>(req, "x-chunk-metadata");

        var result = await _projectService.UploadTrainedModelChunk(stream, metadata?.Name, metadata?.Size, projectId);
        
        return await ResultToResponse(result, req);
    }
    
    [Function("getTrainedModels")]
    public async Task<HttpResponseData> GetTrainedModels(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "project/{projectId}/trained-model")] HttpRequestData req, int projectId)
    {
        var result = await _projectService.GetTrainedModels(projectId);
        
        return await ResultToResponse(result, req);
    }
}
