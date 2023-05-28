using Ardalis.Specification;
using RealtimeCv.Core.Entities;

namespace RealtimeCv.Core.Specifications
{
  public class TrainedModelByNameSpec
      : Specification<TrainedModel>, ISingleResultSpecification<TrainedModel>
  {
    public TrainedModelByNameSpec(string name)
    {
        Query.Where(tm => tm.Name == name)
            .Take(1)
            .EnableCache(nameof(TrainedModelByNameSpec), name);
    }
  }
}
