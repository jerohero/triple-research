using System.Collections.Generic;
using System.Text.Json.Serialization;
using Ardalis.GuardClauses;

namespace RealtimeCv.Infrastructure.Entities;

public class VisionSet : BaseEntity
{
    public string Name { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public ICollection<string> Sources { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public ICollection<string> Models { get; set; }
    
    public VisionSet(string name, ICollection<string> sources, ICollection<string> models)
    {
        Sources = sources;
        Models = models;
        Name = Guard.Against.NullOrEmpty(name, nameof(name));
    }
    
    public void UpdateName(string newName)
    {
        Name = Guard.Against.NullOrEmpty(newName, nameof(newName));
    }
}
