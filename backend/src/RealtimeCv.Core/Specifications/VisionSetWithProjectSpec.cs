using Ardalis.Specification;
using RealtimeCv.Core.Entities;

namespace RealtimeCv.Core.Specifications
{
  public class VisionSetWithProjectSpec
      : Specification<VisionSet>, ISingleResultSpecification<VisionSet>
  {
    public VisionSetWithProjectSpec(int id)
    {
        Query.Where(vs => vs.Id == id)
            .Include(vs => vs.Project)
            .Take(1)
            .EnableCache(nameof(VisionSetWithProjectSpec), id);
    }
  }
}
