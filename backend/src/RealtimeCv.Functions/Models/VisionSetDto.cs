using Newtonsoft.Json;
using RealtimeCv.Core.Entities;

namespace RealtimeCv.Functions.Models;

public class VisionSetDto
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
    public int ProjectId { get; set; }
    
    [JsonRequired]
    public TrainedModelDto TrainedModel { get; set; }

    public VisionSetDto(int id, string name, string[] sources, string containerImage, int projectId, TrainedModelDto trainedModel)
    {
        Id = id;
        Name = name;
        Sources = sources;
        ContainerImage = containerImage;
        ProjectId = projectId;
        TrainedModel = trainedModel;
    }
}
