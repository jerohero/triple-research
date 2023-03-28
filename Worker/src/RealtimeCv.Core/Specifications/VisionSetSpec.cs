using Ardalis.Specification;

namespace RealtimeCv.Core.Specifications;

public class VisionSetSpec : Specification<Entities.VisionSet>
{
    public VisionSetSpec()
    {
        Query.Take(10);
    }
}
