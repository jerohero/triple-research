using Ardalis.Specification.EntityFrameworkCore;
using RealtimeCv.Core.Entities;
using RealtimeCv.Core.Interfaces;

namespace RealtimeCv.Infrastructure.Data.Repositories;

public class VisionSetRepository : RepositoryBase<VisionSet>, IVisionSetRepository
{
  private readonly AppDbContext _dbContext;

  public VisionSetRepository(AppDbContext dbContext) : base(dbContext)
  {
    _dbContext = dbContext;
  }
}
