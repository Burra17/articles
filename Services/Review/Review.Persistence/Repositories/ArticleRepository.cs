namespace Review.Persistence.Repositories;

public class ArticleRepository(ReviewDbContext dbContext)
    : Repository<Article>(dbContext)
{
}
