using System.Collections.Generic;
using Newtonsoft.Json;

namespace RealtimeCv.Core.Models.Dto;

public class ProjectsDto
{
    [JsonRequired]
    public int Id { get; set; }

    [JsonRequired]
    public string Name { get; set; }

    [JsonRequired]
    public ICollection<TrainedModelDto> TrainedModels { get; set; }
    
    public ProjectsDto(int id, string name, ICollection<TrainedModelDto> trainedModels)
    {
        Id = id;
        Name = name;
        TrainedModels = trainedModels;
    }
}
