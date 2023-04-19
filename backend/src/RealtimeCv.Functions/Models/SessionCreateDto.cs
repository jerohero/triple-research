using Newtonsoft.Json;

namespace RealtimeCv.Functions.Models;

public class SessionCreateDto
{
    [JsonRequired]
    public int VisionSetId { get; set; }
    
    [JsonRequired]
    public string Source { get; set; }

    public SessionCreateDto(int visionSetId, string source)
    {
        VisionSetId = visionSetId;
        Source = source;
    }
}
