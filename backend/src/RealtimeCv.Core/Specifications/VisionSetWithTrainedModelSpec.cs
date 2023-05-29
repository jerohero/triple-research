using Ardalis.Specification;
using RealtimeCv.Core.Entities;

namespace RealtimeCv.Core.Specifications
{
  public class VisionSetWithTrainedModelSpec
      : Specification<VisionSet>, ISingleResultSpecification<VisionSet>
  {
    public VisionSetWithTrainedModelSpec(int id)
    {
        Query.Where(vs => vs.Id == id)
            .Include(vs => vs.TrainedModel)
            .Take(1)
            .EnableCache(nameof(SessionWithVisionSetSpec), id);
    }
  }
}
