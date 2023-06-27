using System.Collections.Generic;
using Newtonsoft.Json;

namespace RealtimeCv.Core.Models.Dto;

public class ProjectDto
{
    [JsonRequired]
    public int Id { get; set; }

    [JsonRequired]
    public string Name { get; set; }
    
    [JsonRequired]
    public ICollection<VisionSetDto> VisionSets { get; set; }

    [JsonRequired]
    public ICollection<TrainedModelDto> TrainedModels { get; set; }

    public ProjectDto(int id, string name, ICollection<VisionSetDto> visionSets, ICollection<TrainedModelDto> trainedModels)
    {
        Id = id;
        Name = name;
        VisionSets = visionSets;
        TrainedModels = trainedModels;
    }
}
