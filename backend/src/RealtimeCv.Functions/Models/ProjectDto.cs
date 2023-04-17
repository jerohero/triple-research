using System.Collections.Generic;
using Newtonsoft.Json;
using RealtimeCv.Core.Entities;

namespace RealtimeCv.Functions.Models;

public class ProjectDto
{
    [JsonRequired]
    public int Id { get; set; }

    [JsonRequired]
    public string Name { get; set; }
    
    [JsonRequired]
    public ICollection<VisionSet> VisionSets { get; set; }

    public ProjectDto(int id, string name, ICollection<VisionSet> visionSets)
    {
        Id = id;
        Name = name;
        VisionSets = visionSets;
    }
}
