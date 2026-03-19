using Blocks.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Auth.Persistence;

public sealed class AuthDbContextDesignTimeFactory : DesignTimeFactoryBase<AuthDBContext>
{
    protected override void ConfigureProvider(DbContextOptionsBuilder<AuthDBContext> b, string cs)
        => b.UseSqlServer(cs);
}
