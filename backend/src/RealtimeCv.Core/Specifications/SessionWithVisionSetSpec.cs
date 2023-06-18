using Ardalis.Specification;
using RealtimeCv.Core.Entities;

namespace RealtimeCv.Core.Specifications
{
  public class SessionWithVisionSetSpec
      : Specification<Session>, ISingleResultSpecification<Session>
  {
    public SessionWithVisionSetSpec(int id)
    {
        Query.Where(s => s.Id == id)
            .Include(s => s.VisionSet)
            .Include(s => s.VisionSet.Project)
            .OrderBy(s => s.Id)
            .Take(1)
            .EnableCache(nameof(SessionWithVisionSetSpec), id);
    }
  }
}
