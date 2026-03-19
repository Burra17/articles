using Blocks.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Submission.Persistence;

public sealed class SubmissionDbContextDesignTimeFactory
    : DesignTimeFactoryBase<SubmissionDbContext>
{
    protected override void ConfigureProvider(DbContextOptionsBuilder<SubmissionDbContext> b, string cs)
        => b.UseSqlServer(cs);

    protected override SubmissionDbContext CreateContext(DbContextOptions<SubmissionDbContext> options)
        => new SubmissionDbContext(options, new MemoryCache(new MemoryCacheOptions()));
}
