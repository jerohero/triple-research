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
    private readonly IKubernetes _kubernetes;

    public VisionSetController(
      ILoggerAdapter<VisionSetController> logger,
      IVisionSetService visionSetService, 
      IKubernetes kubernetes
    )
    {
        _logger = logger;
        _visionSetService = visionSetService;
        _kubernetes = kubernetes;
    }

    [Function("getVisionSet")]
    public async Task<HttpResponseData> GetVisionSet(
      [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "visionSet/{visionSetId}")] HttpRequestData req, int visionSetId)
    {
        var result = await _visionSetService.GetVisionSetById(visionSetId);

        return await ResultToResponse(result, req);
    }

    [Function("getVisionSets")]
    public async Task<HttpResponseData> GetVisionSets(
      [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "visionSet")] HttpRequestData req)
    {
        var result = await _visionSetService.GetVisionSets();

        return await ResultToResponse(result, req);
    }

    [Function("createVisionSet")]
    public async Task<HttpResponseData> CreateVisionSet(
      [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "visionSet")] HttpRequestData req)
    {
        var visionSetCreateDto = DeserializeJson<VisionSetCreateDto>(req.Body);

        var result = await _visionSetService.CreateVisionSet(visionSetCreateDto);

        return await ResultToResponse(result, req);
    }

    [Function("updateVisionSet")]
    public async Task<HttpResponseData> UpdateVisionSet(
      [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "visionSet")] HttpRequestData req)
    {
        var visionSetDto = DeserializeJson<VisionSetDto>(req.Body);

        var result = await _visionSetService.UpdateVisionSet(visionSetDto);

        return await ResultToResponse(result, req);
    }

    [Function("deleteVisionSet")]
    public async Task<HttpResponseData> DeleteVisionSet(
      [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "visionSet/{visionSetId}")] HttpRequestData req, int visionSetId)
    {
        Result<VisionSetDto> result = await _visionSetService.DeleteVisionSet(visionSetId);

        return await ResultToResponse(result, req);
    }
}
