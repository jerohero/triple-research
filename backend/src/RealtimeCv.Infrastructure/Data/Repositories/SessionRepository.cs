using Ardalis.Specification.EntityFrameworkCore;
using RealtimeCv.Core.Entities;
using RealtimeCv.Core.Interfaces;

namespace RealtimeCv.Infrastructure.Data.Repositories;

// TODO: These classes can probably be merged if they stay empty

public class SessionRepository : RepositoryBase<Session>, ISessionRepository
{
    private readonly AppDbContext _dbContext;

    public SessionRepository(AppDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
