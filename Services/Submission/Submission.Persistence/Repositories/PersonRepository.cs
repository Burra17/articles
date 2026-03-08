using Microsoft.EntityFrameworkCore;
using Submission.Domain.Entities;

namespace Submission.Persistence.Repositories;

public class PersonRepository(SubmissionDbContext dbContext) 
    : Repository<Person>(dbContext)
{
    public async Task<Person?> GetByUserIdAsync(int userId)
        => await _entity.SingleOrDefaultAsync(x => x.UserId == userId);
}
