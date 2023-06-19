using System;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace RealtimeCv.Core.Models.Dto;

public class SessionDto
{
    [JsonRequired]
    public int Id { get; set; }
    
    [JsonRequired]
    public int VisionSetId { get; set; }

    [JsonRequired]
    public string Source { get; set; }
    
    [JsonRequired]
    public string Pod { get; set; }

    [JsonRequired]
    public DateTime CreatedAt { get; set; }
    
    public DateTime StartedAt { get; set; }
    
    public DateTime StoppedAt { get; set; }
    
    public string Status { get; set; }

    public SessionDto()
    {
    }
    
    public SessionDto(
        int id, int visionSetId, string source, string pod,
        DateTime createdAt, DateTime startedAt, DateTime stoppedAt,
        string status
    )
    {
        Id = id;
        VisionSetId = visionSetId;
        Source = source;
        Pod = pod;
        CreatedAt = createdAt;
        StartedAt = startedAt;
        StoppedAt = stoppedAt;
        Status = status;
    }
}
