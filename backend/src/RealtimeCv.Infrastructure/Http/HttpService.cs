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
/// </summary>
public class HttpService : IHttpService
{
    private readonly HttpClient _httpClient;
    
    public HttpService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<int> GetUrlResponseStatusCode(string url)
    {
        var result = await _httpClient.GetAsync(url);

        return (int)result.StatusCode;
    }

    public async Task<HttpResponseMessage> PostFile(string url, byte[] file, string name = "file")
    {
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

        var response = await _httpClient.PostAsync(url, content);

        return response;
    }

    public async Task Post(string url)
    {
        var response = await _httpClient.PostAsync(url, null);

        var responseString = await response.Content.ReadAsStringAsync();
    }
}
