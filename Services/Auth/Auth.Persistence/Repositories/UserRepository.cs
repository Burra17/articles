using Auth.Domain.Users;
using Blocks.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Auth.Persistence.Repositories;

public class UserRepository(AuthDBContext dbContext)
    : RepositoryBase<AuthDBContext, User>(dbContext)
{
    private IQueryable<User> QueryWithPerson()
        => base.Query().Include(u => u.Person);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await QueryWithPerson().SingleOrDefaultAsync(u => u.Email == email, ct);

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
        => await QueryWithPerson()
            .Include(u => u.RefreshTokens)
            .SingleOrDefaultAsync(u => u.RefreshTokens.Any(rt =>
                rt.Token == refreshToken &&
                rt.RevokedOn == null &&
                rt.ExpiresOn > DateTime.UtcNow), ct);

    public async Task<bool> ExistsAsync(string email, CancellationToken ct = default)
        => await QueryWithPerson().AnyAsync(u => u.Email == email, ct);
}
