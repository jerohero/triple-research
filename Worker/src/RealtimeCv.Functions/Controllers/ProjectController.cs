using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;
using RealtimeCv.Core.Entities;
using RealtimeCv.Core.Interfaces;
using RealtimeCv.Core.Specifications;
using RealtimeCv.Functions.Interfaces;
using RealtimeCv.Functions.Models;

namespace RealtimeCv.Functions.Controllers;

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

  [Function("createProject")]
  public async Task<HttpResponseData> CreateProject(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "project")] HttpRequestData req)
  {
    ProjectCreateDto? projectCreateDto = DeserializeJson<ProjectCreateDto>(req.Body);
    
    // TODO validation

    if (projectCreateDto == null)
    {
      return await CreateJsonResponse(req, HttpStatusCode.BadRequest, "Missing DTO");
    }

    _logger.LogInformation("Name: " + projectCreateDto.Name);
    ProjectDto created = await _projectService.CreateProject(projectCreateDto);

    return await CreateJsonResponse(req, HttpStatusCode.Created, created);
  }
  
  [Function("updateProject")]
  public async Task<HttpResponseData> UpdateProject(
    [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "project")] HttpRequestData req)
  {
    ProjectDto? projectDto = DeserializeJson<ProjectDto>(req.Body);
    
    // TODO validation

    if (projectDto == null)
    {
      return await CreateJsonResponse(req, HttpStatusCode.BadRequest, "Missing DTO");
    }

    ProjectDto updated = await _projectService.UpdateProject(projectDto);

    return await CreateJsonResponse(req, HttpStatusCode.OK, updated);
  }
}
