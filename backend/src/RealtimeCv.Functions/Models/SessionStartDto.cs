using Newtonsoft.Json;

namespace RealtimeCv.Functions.Models;

public class SessionStartDto
{
    [JsonRequired]
    public int VisionSetId { get; set; }
    
    [JsonRequired]
    public string Source { get; set; }

    public SessionStartDto(int visionSetId, string source)
    {
        VisionSetId = visionSetId;
        Source = source;
    }
}
