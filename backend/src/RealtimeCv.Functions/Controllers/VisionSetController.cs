using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Ardalis.Result;
using k8s;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using RealtimeCv.Core.Interfaces;
using RealtimeCv.Functions.Interfaces;
using RealtimeCv.Functions.Models;

namespace RealtimeCv.Functions.Controllers;

/// <summary>
/// Controller for Project related endpoints.
/// </summary>
public class VisionSetController : BaseController
{
    private readonly ILoggerAdapter<VisionSetController> _logger;
    private readonly IVisionSetService _visionSetService;

    public VisionSetController(
      ILoggerAdapter<VisionSetController> logger,
      IVisionSetService visionSetService
    )
    {
        _logger = logger;
        _visionSetService = visionSetService;
    }

    [Function("getVisionSet")]
    public async Task<HttpResponseData> GetVisionSet(
      [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "vision-set/{visionSetId}")] HttpRequestData req, int projectId, int visionSetId)
    {
        var result = await _visionSetService.GetVisionSetById(visionSetId);

        return await ResultToResponse(result, req);
    }

    [Function("getVisionSetsByProject")]
    public async Task<HttpResponseData> GetVisionSetsByProject(
      [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "project/{projectId}/vision-set")] HttpRequestData req, int projectId)
    {
        var result = await _visionSetService.GetVisionSetsByProject(projectId);

        return await ResultToResponse(result, req);
    }

    [Function("createVisionSet")]
    public async Task<HttpResponseData> CreateVisionSet(
      [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "project/{projectId}/vision-set")] HttpRequestData req, int projectId)
    {
        var visionSetCreateDto = DeserializeJson<VisionSetCreateDto>(req.Body);

        var result = await _visionSetService.CreateVisionSet(visionSetCreateDto, projectId);

        return await ResultToResponse(result, req);
    }

    [Function("updateVisionSet")]
    public async Task<HttpResponseData> UpdateVisionSet(
      [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "vision-set")] HttpRequestData req)
    {
        var visionSetDto = DeserializeJson<VisionSetDto>(req.Body);

        var result = await _visionSetService.UpdateVisionSet(visionSetDto);

        return await ResultToResponse(result, req);
    }

    [Function("deleteVisionSet")]
    public async Task<HttpResponseData> DeleteVisionSet(
      [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "vision-set/{visionSetId}")] HttpRequestData req, int visionSetId)
    {
        Result<VisionSetDto> result = await _visionSetService.DeleteVisionSet(visionSetId);

        return await ResultToResponse(result, req);
    }
}
