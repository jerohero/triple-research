using Ardalis.Specification;
using RealtimeCv.Core.Entities;

namespace RealtimeCv.Core.Specifications
{
  public class VisionSetsWithTrainedModelByProject
      : Specification<VisionSet>
  {
    public VisionSetsWithTrainedModelByProject(int projectId)
    {
        Query.Where(vs => vs.ProjectId == projectId)
            .Include(vs => vs.TrainedModel)
            .OrderBy(vs => vs.Id)
            .EnableCache(nameof(VisionSetsWithTrainedModelByProject), projectId);
    }
  }
}
