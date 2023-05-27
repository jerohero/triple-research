using System.Net.Http;
using System.Threading.Tasks;

namespace RealtimeCv.Core.Interfaces;

public interface IHttpService
{
    Task<int> GetUrlResponseStatusCode(string url);
    Task<HttpResponseMessage> Post(string url, HttpContent content);
    Task<HttpContent> ImageToHttpContent(byte[] input);
    HttpContent StringToHttpContent(string text);
}
