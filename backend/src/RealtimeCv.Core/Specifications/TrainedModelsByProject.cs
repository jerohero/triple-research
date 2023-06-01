using Ardalis.Specification;
using RealtimeCv.Core.Entities;

namespace RealtimeCv.Core.Specifications
{
  public class TrainedModelsByProject
      : Specification<TrainedModel>
  {
    public TrainedModelsByProject(int projectId)
    {
        Query.Where(tm => tm.ProjectId == projectId)
            .EnableCache(nameof(TrainedModelByNameSpec), projectId);
    }
  }
}
