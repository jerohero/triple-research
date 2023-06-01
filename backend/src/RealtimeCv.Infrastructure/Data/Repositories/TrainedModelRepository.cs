using Ardalis.Specification.EntityFrameworkCore;
using RealtimeCv.Core.Entities;
using RealtimeCv.Core.Interfaces;

namespace RealtimeCv.Infrastructure.Data.Repositories;

public class TrainedModelRepository : RepositoryBase<TrainedModel>, ITrainedModelRepository
{
    private readonly AppDbContext _dbContext;

    public TrainedModelRepository(AppDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
