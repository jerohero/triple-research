using Newtonsoft.Json;

namespace RealtimeCv.Functions.Models;

public class VisionSetUpdateDto
{
    [JsonRequired]
    public int Id { get; set; }

    [JsonRequired]
    public string Name { get; set; }
    
    [JsonRequired]
    public string[] Sources { get; set; }
    
    [JsonRequired]
    public int TrainedModelId { get; set; }

    public VisionSetUpdateDto(int id, string name, string[] sources, int trainedModelId)
    {
        Id = id;
        Name = name;
        Sources = sources;
        TrainedModelId = trainedModelId;
    }
}
