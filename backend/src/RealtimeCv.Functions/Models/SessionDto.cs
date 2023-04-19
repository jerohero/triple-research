using System;
using Newtonsoft.Json;

namespace RealtimeCv.Functions.Models;

public class SessionDto
{
    [JsonRequired]
    public int Id { get; set; }
    
    [JsonRequired]
    public int VisionSetId { get; set; }

    [JsonRequired]
    public bool IsActive { get; set; }
    
    [JsonRequired]
    public string Source { get; set; }

    [JsonRequired]
    public DateTime StartedAt { get; set; }
    
    [JsonRequired]
    public DateTime StoppedAt { get; set; }

    public SessionDto(int id, int visionSetId, string sources, bool isActive, DateTime startedAt, DateTime stoppedAt)
    {
        Id = id;
        VisionSetId = visionSetId;
        Source = sources;
        IsActive = isActive;
        StartedAt = startedAt;
        StoppedAt = stoppedAt;
    }
}
