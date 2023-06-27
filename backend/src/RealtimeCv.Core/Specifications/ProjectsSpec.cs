using Ardalis.Specification;
using RealtimeCv.Core.Entities;

namespace RealtimeCv.Core.Specifications
{
  public class ProjectsSpec
      : Specification<Project>
  {
    public ProjectsSpec()
    {
        Query.Include(p => p.TrainedModels)
            .OrderBy(p => p.Id)
            .EnableCache(nameof(ProjectsSpec));
    }
  }
}
