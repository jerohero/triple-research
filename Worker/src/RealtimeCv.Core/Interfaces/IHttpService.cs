using System.Net.Http;
using System.Threading.Tasks;

namespace RealtimeCv.Core.Interfaces;

public interface IHttpService
{
  Task<int> GetUrlResponseStatusCodeAsync(string url);
  Task<HttpResponseMessage> PostFileAsync(string url, byte[] file, string name = "file");
  Task PostAsync(string url);
}
