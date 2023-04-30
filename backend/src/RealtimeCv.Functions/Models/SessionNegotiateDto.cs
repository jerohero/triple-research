using Newtonsoft.Json;

namespace RealtimeCv.Functions.Models;

public class SessionNegotiateDto
{
    [JsonRequired]
    public string Url { get; set; }

    public SessionNegotiateDto(string url)
    {
        Url = url;
    }
}
