using Newtonsoft.Json;

namespace RealtimeCv.Functions.Models;

public class VisionSetCreateDto
{
    [JsonRequired]
    public int ProjectId { get; set; }
    
    [JsonRequired]
    public string Name { get; set; }
    
    [JsonRequired]
    public string[] Sources { get; set; }

    public VisionSetCreateDto(int projectId, string name, string[] sources)
    {
        ProjectId = projectId;
        Name = name;
        Sources = sources;
    }
}
