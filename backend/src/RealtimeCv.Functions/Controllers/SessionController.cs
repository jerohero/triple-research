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
/// Controller for Session related endpoints.
/// </summary>
public class SessionController : BaseController
{
    private readonly ILoggerAdapter<SessionController> _logger;
    private readonly ISessionService _sessionService;

    public SessionController(
      ILoggerAdapter<SessionController> logger,
      ISessionService sessionService
    )
    {
        _logger = logger;
        _sessionService = sessionService;
    }

    [Function("getSession")]
    public async Task<HttpResponseData> GetSession(
      [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "session/{sessionId}")] HttpRequestData req, int sessionId)
    {
        var result = await _sessionService.GetSessionById(sessionId);

        return await ResultToResponse(result, req);
    }

    [Function("getSessionsByVisionSet")]
    public async Task<HttpResponseData> GetSessionsByVisionSet(
      [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "vision-set/{visionSetId}/session")] HttpRequestData req, int visionSetId)
    {
        var result = await _sessionService.GetSessionsByVisionSet(visionSetId);

        return await ResultToResponse(result, req);
    }

    // [Function("createSession")]
    // public async Task<HttpResponseData> CreateSession(
    //   [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "vision-set")] HttpRequestData req)
    // {
    //     var sessionCreateDto = DeserializeJson<SessionCreateDto>(req.Body);
    //
    //     var result = await _sessionService.CreateSession(sessionCreateDto);
    //
    //     return await ResultToResponse(result, req);
    // }

    // [Function("updateSession")]
    // public async Task<HttpResponseData> UpdateSession(
    //   [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "vision-set")] HttpRequestData req)
    // {
    //     var sessionDto = DeserializeJson<SessionDto>(req.Body);
    //
    //     var result = await _sessionService.UpdateSession(sessionDto);
    //
    //     return await ResultToResponse(result, req);
    // }

    [Function("deleteSession")]
    public async Task<HttpResponseData> DeleteSession(
      [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "session/{sessionId}")] HttpRequestData req, int sessionId)
    {
        Result<SessionDto> result = await _sessionService.DeleteSession(sessionId);

        return await ResultToResponse(result, req);
    }
    
    [Function("stopSession")]
    public async Task<HttpResponseData> StopSession(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "session/{sessionId}/stop")] HttpRequestData req, int sessionId)
    {
        Result<SessionDto> result = await _sessionService.StopSession(sessionId);

        return await ResultToResponse(result, req);
    }
    
    [Function("negotiate")]
    public async Task<HttpResponseData> NegotiateSession(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "session/{sessionId}/negotiate")] HttpRequestData req, int sessionId)
    {
        var result = await _sessionService.NegotiateSession(sessionId);

        return await ResultToResponse(result, req);
    }
}
