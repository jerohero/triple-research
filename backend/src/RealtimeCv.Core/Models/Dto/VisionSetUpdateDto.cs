using Newtonsoft.Json;

namespace RealtimeCv.Core.Models.Dto;

public class VisionSetUpdateDto
{
    [JsonRequired]
    public int Id { get; set; }

    [JsonRequired]
    public string Name { get; set; }
    
    [JsonRequired]
    public string[] Sources { get; set; }
    
    [JsonRequired]
    public string ContainerImage { get; set; }
    
    [JsonRequired]
    public int TrainedModelId { get; set; }

    public VisionSetUpdateDto(int id, string name, string[] sources, string containerImage, int trainedModelId)
    {
        Id = id;
        Name = name;
        Sources = sources;
        ContainerImage = containerImage;
        TrainedModelId = trainedModelId;
    }
}
