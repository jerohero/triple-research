using System.Net.Http;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace RealtimeCv.Core.Interfaces;

public interface IHttpService
{
    Task<int> GetUrlResponseStatusCode(string url);
    Task<HttpResponseMessage> Post(string url, HttpContent content = null);
    Task<HttpContent> ImageToHttpContent(byte[] input);
    HttpContent StringToHttpContent(string text);
}
