using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using RealtimeCv.Core.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace RealtimeCv.Infrastructure.Http;

/// <summary>
/// An implementation of IHttpService using HttpClient
/// TODO https://learn.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests#what-is-httpclientfactory
/// </summary>
public class HttpService : IHttpService
{
  public async Task<int> GetUrlResponseStatusCodeAsync(string url)
  {
    using HttpClient client = new HttpClient();

    HttpResponseMessage result = await client.GetAsync(url);

    return (int)result.StatusCode;
  }

  public async Task<HttpResponseMessage> PostFileAsync(string url, byte[] file, string name = "file")
  {
    using HttpClient client = new HttpClient();

    using MemoryStream ms = new MemoryStream();

    Image img = Image.Load<Rgba32>(file);
    await img.SaveAsJpegAsync(ms);

    byte[] bits = ms.ToArray();

    using var content = new MultipartFormDataContent(
      "Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture)
    );

    content.Add(new ByteArrayContent(bits), "file", "upload.png");

    HttpResponseMessage response = await client.PostAsync(url, content);

    return response;
  }

  public async Task PostAsync(string url)
  {
    using HttpClient client = new HttpClient();

    HttpResponseMessage response = await client.PostAsync(url, null);

    string responseString = await response.Content.ReadAsStringAsync();
  }
}
