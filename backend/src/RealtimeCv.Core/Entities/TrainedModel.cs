namespace RealtimeCv.Core.Entities;

public class TrainedModel : BaseEntity
{
    public Project Project { get; set; }
    
    public string Name { get; set; }

    public int ProjectId { get; set; }
}
