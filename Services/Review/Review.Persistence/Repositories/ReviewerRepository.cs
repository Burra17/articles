using Review.Domain.Reviewers;

namespace Review.Persistence.Repositories;

public class ReviewerRepository(ReviewDbContext dbContext)
    : Repository<Reviewer>(dbContext)
{
    public async Task<Reviewer?> GetByUserIdAsync(int userId, CancellationToken ct = default)
        => await Entity.SingleOrDefaultAsync(r => r.UserId == userId);

    public async Task<Reviewer?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await Entity
            .SingleOrDefaultAsync(e => e.Email.Value.ToLower() == email.ToLower(), ct);
}
