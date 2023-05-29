using System.Collections.Generic;
using Ardalis.GuardClauses;

namespace RealtimeCv.Core.Entities;

public class Project : BaseEntity
{
    public string Name { get; set; }
    
    public ICollection<VisionSet> VisionSets { get; set; }
    
    public ICollection<TrainedModel> TrainedModels { get; set; }
}
