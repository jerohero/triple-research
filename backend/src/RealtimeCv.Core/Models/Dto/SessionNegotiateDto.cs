using Newtonsoft.Json;

namespace RealtimeCv.Core.Models.Dto;

public class SessionNegotiateDto
{
    [JsonRequired]
    public string Url { get; set; }

    public SessionNegotiateDto(string url)
    {
        Url = url;
    }
}
