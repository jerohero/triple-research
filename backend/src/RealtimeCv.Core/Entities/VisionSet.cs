using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Ardalis.GuardClauses;

namespace RealtimeCv.Core.Entities;

public class VisionSet : BaseEntity
{
    public string Name { get; set; }
    
    public Project Project { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public ICollection<string> Sources { get; set; }
    
    public void UpdateName(string newName)
    {
        Name = Guard.Against.NullOrEmpty(newName, nameof(newName));
    }
}
