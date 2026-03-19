using Blocks.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Review.Persistence;

public sealed class ReviewDbContextDesignTimeFactory
    : DesignTimeFactoryBase<ReviewDbContext>
{
    protected override void ConfigureProvider(DbContextOptionsBuilder<ReviewDbContext> b, string cs)
        => b.UseSqlServer(cs);

    protected override ReviewDbContext CreateContext(DbContextOptions<ReviewDbContext> options)
        => new ReviewDbContext(options);
}
