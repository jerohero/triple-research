using Ardalis.Specification.EntityFrameworkCore;
using RealtimeCv.Core.Entities;
using RealtimeCv.Core.Interfaces;

namespace RealtimeCv.Infrastructure.Data.Repositories;

public class ProjectRepository : RepositoryBase<Project>, IProjectRepository
{
    private readonly AppDbContext _dbContext;

    public ProjectRepository(AppDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
