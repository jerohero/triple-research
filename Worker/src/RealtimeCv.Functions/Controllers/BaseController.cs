using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace RealtimeCv.Functions.Controllers;

public abstract class BaseController
{
  protected async Task<HttpResponseData> CreateJsonResponse(HttpRequestData requestData, HttpStatusCode statusCode, object? jsonObject)
  {
    var response = requestData.CreateResponse();
    
    if (jsonObject is not null)
      await response.WriteAsJsonAsync(jsonObject);
    
    response.StatusCode = statusCode;
    
    return response;
  }
  
  // protected async Task<HttpResponseData> CreateErrorResponse(HttpRequestData requestData, HttpStatusCode statusCode, string message = "")
  // {
  //   // Create a response in one line to keep the controller readable. 
  //   var response = requestData.CreateResponse(statusCode);
  //   if (!String.IsNullOrEmpty(message))
  //   {
  //     await response.WriteStringAsync(message);
  //   }
  //   return response;
  // }
  //
  // protected async Task<HttpResponseData> CreateErrorResponse(HttpRequestData requestData, HttpStatusCode statusCode, List<ValidationFailure> errors)
  // {
  //   // Setup a response with all Errors noted by the FluentValidation
  //   var response = requestData.CreateResponse(statusCode);
  //   string message = "";
  //   errors.ForEach(e => message += $"Property: {e.PropertyName}. Problem: {e.ErrorMessage}\n");
  //   return await CreateErrorResponse(requestData, statusCode, message);
  // }
  // protected T GetSerializedJsonObject<T>(string json) where T : class
  // {
  //   // simple deserializer method so we can handle faulty json objects in the Validator.  
  //   try
  //   {
  //     return JsonConvert.DeserializeObject<T>(json);
  //   }
  //   catch
  //   {
  //     return null;
  //   }
  // }
  //
  // protected T GetDeserializedJsonObject<T>(Stream body) where T : class
  // {
  //   return GetSerializedJsonObject<T>(new StreamReader(body).ReadToEnd());
  // }
  // protected UserIdentityResult GetUserIdentityResult(FunctionContext context, HttpRequestData req, Role[] AuthorizedRoles = null)
  // {
  //   // Given a Login Context, request, and array of Roles that can access this call:
  //   // - Return UserId and UserRole objects
  //   // - Return HttpResponseData object if authorization is invalid (403 Forbidden or 401 Unauthorized)
  //   AuthorizedRoles ??= new Role[4] { Role.Student, Role.Instructor, Role.Admin, Role.SuperAdmin };
  //
  //   UserIdentityResult result = new UserIdentityResult();
  //   ClaimsPrincipal loggedInUser = context.GetUser();
  //
  //   if (object.Equals(loggedInUser, null))
  //     result.ResponseMessage = req.CreateResponse(HttpStatusCode.Forbidden);
  //   else
  //   {
  //     int loggedInUserId;
  //     Role loggedInUserRole;
  //     try
  //     {
  //       loggedInUserId = int.Parse(loggedInUser.Claims.Where(c => c.Type == ClaimTypes.PrimarySid).First().Value);
  //       loggedInUserRole = Enum.Parse<Role>(loggedInUser.Claims.Where(c => c.Type == ClaimTypes.Role).First().Value);
  //       if (!AuthorizedRoles.Contains(loggedInUserRole))
  //         result.ResponseMessage = req.CreateResponse(HttpStatusCode.Unauthorized);
  //
  //       result.UserId = loggedInUserId;
  //       result.Role = loggedInUserRole;
  //     }
  //     catch
  //     {
  //       result.ResponseMessage = req.CreateResponse(HttpStatusCode.Forbidden);
  //     }
  //   }
  //   return result;
  // }
}
