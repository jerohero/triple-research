using Ardalis.Specification;
using RealtimeCv.Core.Entities;

namespace RealtimeCv.Core.Specifications
{
  public class ProjectSpec
      : Specification<Project>, ISingleResultSpecification<Project>
  {
    public ProjectSpec(int id)
    {
        Query.Where(p => p.Id == id)
            .Include(p => p.VisionSets)
            .Include(p => p.TrainedModels)
            .OrderBy(p => p.Id)
            .Take(1)
            .EnableCache(nameof(ProjectSpec), id);
    }
  }
}
