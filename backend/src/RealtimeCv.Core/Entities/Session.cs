using System;

namespace RealtimeCv.Core.Entities;

public class Session : BaseEntity
{
    public VisionSet VisionSet { get; set; }
    
    public int VisionSetId { get; set; }
    
    public string Source { get; set; }
    
    public string Pod { get; set; }
    
    public bool IsActive { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime StartedAt { get; set; }
    
    public DateTime StoppedAt { get; set; }
}
