using System.Net.Http;
using System.Threading.Tasks;

namespace RealtimeCv.Core.Interfaces;

public interface IHttpService
{
    Task<int> GetUrlResponseStatusCode(string url);
    Task<HttpResponseMessage> PostFile(string url, byte[] file, string name = "file");
    Task Post(string url);
}
