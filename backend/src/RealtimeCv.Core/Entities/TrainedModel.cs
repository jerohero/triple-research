using System.Collections.Generic;

namespace RealtimeCv.Core.Entities;

public class TrainedModel : BaseEntity
{
    public Project Project { get; set; }
    
    public string Name { get; set; }
    
    public bool IsUploadFinished { get; set; }

    public int ProjectId { get; set; }
    
    public ICollection<VisionSet> VisionSets { get; set; }
}
