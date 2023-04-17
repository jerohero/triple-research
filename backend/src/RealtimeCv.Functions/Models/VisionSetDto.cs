using Newtonsoft.Json;

namespace RealtimeCv.Functions.Models;

public class VisionSetDto
{
    [JsonRequired]
    public int Id { get; set; }

    [JsonRequired]
    public string Name { get; set; }
    
    [JsonRequired]
    public string[] Sources { get; set; }

    public VisionSetDto(int id, string name, string[] sources)
    {
        Id = id;
        Name = name;
        Sources = sources;
    }
}
