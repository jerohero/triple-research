﻿using System.Threading.Tasks;
using Ardalis.Result;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
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
        var projectDto = DeserializeJson<ProjectDto>(req.Body);

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
}