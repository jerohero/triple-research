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
    public async Task<int> GetUrlResponseStatusCode(string url)
    {
        using var client = new HttpClient();

        var result = await client.GetAsync(url);

        return (int)result.StatusCode;
    }

    public async Task<HttpResponseMessage> PostFile(string url, byte[] file, string name = "file")
    {
        using var client = new HttpClient();

        using var ms = new MemoryStream();

        Image img = Image.Load<Rgba32>(file);
        await img.SaveAsJpegAsync(ms);

        var bits = ms.ToArray();

        using var content = new MultipartFormDataContent(
          "Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture)
        )
        {
            { new ByteArrayContent(bits), "file", "upload.png" }
        };

        var response = await client.PostAsync(url, content);

        return response;
    }

    public async Task Post(string url)
    {
        using var client = new HttpClient();

        var response = await client.PostAsync(url, null);

        var responseString = await response.Content.ReadAsStringAsync();
    }
}
