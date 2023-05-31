using System;
using Newtonsoft.Json;

namespace RealtimeCv.Core.Models.Dto;

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
    public string Pod { get; set; }

    [JsonRequired]
    public DateTime CreatedAt { get; set; }
    
    [JsonRequired]
    public DateTime StartedAt { get; set; }
    
    [JsonRequired]
    public DateTime StoppedAt { get; set; }

    public SessionDto(
        int id, int visionSetId, string source, string pod, bool isActive,
        DateTime createdAt, DateTime startedAt, DateTime stoppedAt
    )
    {
        Id = id;
        VisionSetId = visionSetId;
        Source = source;
        Pod = pod;
        IsActive = isActive;
        CreatedAt = createdAt;
        StartedAt = startedAt;
        StoppedAt = stoppedAt;
    }
}
