using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Ardalis.GuardClauses;
using JetBrains.Annotations;

namespace RealtimeCv.Core.Entities;

public class VisionSet : BaseEntity
{
    public Project Project { get; set; }
    
    [CanBeNull]
    public TrainedModel TrainedModel { get; set; }
    
    public string Name { get; set; }

    public int ProjectId { get; set; }
    
    [CanBeNull]
    public int? TrainedModelId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public ICollection<string> Sources { get; set; }
    
    public ICollection<Session> Sessions { get; set; }
}
