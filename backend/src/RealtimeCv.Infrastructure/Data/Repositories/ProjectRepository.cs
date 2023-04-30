using Ardalis.Specification.EntityFrameworkCore;
using RealtimeCv.Core.Interfaces;
using RealtimeCv.Infrastructure.Entities;
using RealtimeCv.Infrastructure.Interfaces;

namespace RealtimeCv.Infrastructure.Data.Repositories;

public class ProjectRepository : RepositoryBase<Project>, IProjectRepository
{
    private readonly AppDbContext _dbContext;

    public ProjectRepository(AppDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
