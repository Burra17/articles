using Review.Domain.Invitations;

namespace Review.Persistence.Repositories;

public class ReviewInvitationRepository(ReviewDbContext dbContext) 
    : Repository<ReviewInvitation>(dbContext)
{
    public async Task<ReviewInvitation> GetByTokenOrThrowAsync(string token, CancellationToken ct = default)
        => await Entity.SingleOrThrowAsync(i => i.Token.Value == token);
}
