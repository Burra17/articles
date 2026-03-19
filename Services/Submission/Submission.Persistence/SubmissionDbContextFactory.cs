using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Caching.Memory;

namespace Submission.Persistence;

public class SubmissionDbContextFactory : IDesignTimeDbContextFactory<SubmissionDbContext>
{
    public SubmissionDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<SubmissionDbContext>()
            .UseSqlServer("Server=localhost,1433;Database=SubmissionDb;User Id=sa;Password=Password123!;TrustServerCertificate=True")
            .Options;

        var cache = new MemoryCache(new MemoryCacheOptions());

        return new SubmissionDbContext(options, cache);
    }
}
