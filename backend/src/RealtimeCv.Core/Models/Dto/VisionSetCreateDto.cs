using Newtonsoft.Json;

namespace RealtimeCv.Core.Models.Dto;

public class VisionSetCreateDto
{
    [JsonRequired]
    public int ProjectId { get; set; }
    
    [JsonRequired]
    public string Name { get; set; }
    
    [JsonRequired]
    public string[] Sources { get; set; }
    
    [JsonRequired]
    public string ContainerImage { get; set; }
    
    [JsonRequired]
    public int TrainedModelId { get; set; }

    public VisionSetCreateDto(int projectId, string name, string[] sources, string containerImage, int trainedModelId)
    {
        ProjectId = projectId;
        Name = name;
        Sources = sources;
        ContainerImage = containerImage;
        TrainedModelId = trainedModelId;
    }
}
