using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Ardalis.Result;
using AutoMapper;
using k8s;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using RealtimeCv.Core.Entities;
using RealtimeCv.Core.Interfaces;
using RealtimeCv.Core.Specifications;
using RealtimeCv.Functions.Interfaces;
using RealtimeCv.Functions.Models;

namespace RealtimeCv.Functions.Controllers;

/// <summary>
/// Controller for VisionSet related endpoints.
/// </summary>
public class VisionSetController : BaseController
{
    private readonly ILoggerAdapter<VisionSetController> _logger;
    private readonly IVisionSetService _visionSetService;
    private readonly ISessionService _sessionService;
    private readonly IKubernetesService _kubernetesService;
    private readonly IMapper _mapper;

    public VisionSetController(
      ILoggerAdapter<VisionSetController> logger,
      IVisionSetService visionSetService,
      ISessionService sessionService,
      IKubernetesService kubernetesService,
      IMapper mapper
    )
    {
        _logger = logger;
        _visionSetService = visionSetService;
        _sessionService = sessionService;
        _kubernetesService = kubernetesService;
        _mapper = mapper;
    }

    [Function("getVisionSet")]
    public async Task<HttpResponseData> GetVisionSet(
      [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "vision-set/{visionSetId}")] HttpRequestData req, int visionSetId)
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
      [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "vision-set")] HttpRequestData req)
    {
        var visionSetCreateDto = DeserializeJson<VisionSetCreateDto>(req.Body);

        var result = await _visionSetService.CreateVisionSet(visionSetCreateDto);

        return await ResultToResponse(result, req);
    }

    [Function("updateVisionSet")]
    public async Task<HttpResponseData> UpdateVisionSet(
      [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "vision-set")] HttpRequestData req)
    {
        var visionSetUpdateDto = DeserializeJson<VisionSetUpdateDto>(req.Body);

        var result = await _visionSetService.UpdateVisionSet(visionSetUpdateDto);

        return await ResultToResponse(result, req);
    }

    [Function("deleteVisionSet")]
    public async Task<HttpResponseData> DeleteVisionSet(
      [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "vision-set/{visionSetId}")] HttpRequestData req, int visionSetId)
    {
        Result<VisionSetDto> result = await _visionSetService.DeleteVisionSet(visionSetId);

        return await ResultToResponse(result, req);
    }

    [Function("startVisionSetSession")]
    public async Task<HttpResponseData> StartVisionSetSession(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "vision-set/{visionSetId}/start")] HttpRequestData req, int visionSetId)
    {
        var startSessionDto = new SessionStartDto(visionSetId, "rtmp://live.restream.io/live/re_6435068_ac960121c66cd1e6a9f5"); // TODO replace with stream url

        var result = await _sessionService.StartSession(startSessionDto);
        
        if (result.Errors.Any())
        {
            return await ResultToResponse(result, req);
        }

        return await ResultToResponse(result, req);
    }
}
