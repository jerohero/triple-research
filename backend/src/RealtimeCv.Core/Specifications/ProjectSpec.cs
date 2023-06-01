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
            .OrderBy(p => p.Id)
            .Take(1)
            .EnableCache(nameof(ProjectSpec), id);
    }
  }
}
