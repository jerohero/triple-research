using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Ardalis.Result;
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;

namespace RealtimeCv.Functions.Controllers;

/// <summary>
/// Base controller that provides common functionality for all controllers.
/// </summary>
public abstract class BaseController
{
    private readonly IDictionary<Enum, HttpStatusCode> _statusCodeDict = new Dictionary<Enum, HttpStatusCode>
  {
    { ResultStatus.Ok, HttpStatusCode.OK },
    { ResultStatus.Forbidden, HttpStatusCode.Forbidden },
    { ResultStatus.Invalid, HttpStatusCode.BadRequest },
    { ResultStatus.NotFound, HttpStatusCode.NotFound },
    { ResultStatus.Error, HttpStatusCode.InternalServerError },
    { ResultStatus.Unauthorized, HttpStatusCode.Unauthorized }
  };

    protected async Task<HttpResponseData> ResultToResponse<T>(Result<T> result, HttpRequestData req)
    {
        if (!result.Status.Equals(ResultStatus.Ok))
        {
            return result.ValidationErrors.Any()
              ? await CreateJsonResponse(req, GetStatusCode(result.Status), result.ValidationErrors)
              : await CreateJsonResponse(req, GetStatusCode(result.Status), result.Errors);
        }

        return await CreateJsonResponse(req, HttpStatusCode.OK, result.Value);
    }

    protected T? DeserializeJson<T>(Stream body) where T : class
    {
        return SerializeJson<T>(new StreamReader(body).ReadToEnd());
    }
    
    protected T? ExtractMetadata<T>(HttpRequestData req, string headerName)
    {
        req.Headers.TryGetValues(headerName, out var metadataHeader);

        return JsonConvert.DeserializeObject<T>(metadataHeader!.First());
    }

    private async Task<HttpResponseData> CreateJsonResponse(HttpRequestData requestData, HttpStatusCode statusCode, object? jsonObject)
    {
        var response = requestData.CreateResponse();

        if (jsonObject is not null)
        {
            await response.WriteAsJsonAsync(jsonObject);
        }

        response.StatusCode = statusCode;

        return response;
    }

    private T? SerializeJson<T>(string json) where T : class
    {
        try
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
        catch
        {
            return null;
        }
    }

    private HttpStatusCode GetStatusCode(ResultStatus status)
    {
        return _statusCodeDict[status];
    }
}
