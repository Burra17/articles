using Auth.Domain.Persons;
using Blocks.Domain.Entities;
using Blocks.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Auth.Persistence.Repositories;

public class PersonRepository(AuthDBContext dbContext) 
    : RepositoryBase<AuthDBContext, Person>(dbContext)
{
    public async Task<Person?> GetByUserIdAsync(int  userId, CancellationToken ct = default)
        => await Query()
        .SingleOrDefaultAsync(e => e.UserId == userId, ct);

    public async Task<Person?> GetByEmailAsync(string email, CancellationToken ct = default)
    => await Query()
    .Include(e => e.User)
    .SingleOrDefaultAsync(e => e.Email.NormalizedEmail == email.ToUpperInvariant(), ct);

    protected override IQueryable<Person> Query()
        => base.Query().Include(p => p.User);
}
