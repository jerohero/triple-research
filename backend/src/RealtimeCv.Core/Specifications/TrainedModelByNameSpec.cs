using Ardalis.Specification;
using RealtimeCv.Core.Entities;

namespace RealtimeCv.Core.Specifications
{
  public class TrainedModelByNameSpec
      : Specification<TrainedModel>, ISingleResultSpecification<TrainedModel>
  {
    public TrainedModelByNameSpec(string name)
    {
        Query.Where(p => p.Name == name)
            .Take(1)
            .EnableCache(nameof(TrainedModelByNameSpec), name);
    }
  }
}
