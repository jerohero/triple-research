using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using RealtimeCv.Core.Entities;
using RealtimeCv.Core.Interfaces;

namespace RealtimeCv.Functions.Controllers;

public class ProjectController : BaseController
{
  private readonly ILoggerAdapter<ProjectController> _logger;
  private readonly IVisionSetRepository _repo;
  
  public ProjectController(
    ILoggerAdapter<ProjectController> logger, 
    IVisionSetRepository repo
  )
  {
    _logger = logger;
    _repo = repo;
  }

  [Function("createProject")]
  public async Task<HttpResponseData> CreateProject(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "project")] HttpRequestData req)
  {
    List<VisionSet> e = await _repo.ListAsync(); 
    _logger.LogInformation("EADAWDAWDADAWDAWDAE: " + e.Count);
    
    return await CreateJsonResponse(req, HttpStatusCode.Created, "HI");
  }
}
