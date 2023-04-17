using Newtonsoft.Json;

namespace RealtimeCv.Functions.Models;

public class VisionSetCreateDto
{
    [JsonRequired]
    public string Name { get; set; }
    
    [JsonRequired]
    public string[] Sources { get; set; }

    public VisionSetCreateDto(string name, string[] sources)
    {
        Name = name;
        Sources = sources;
    }
}
