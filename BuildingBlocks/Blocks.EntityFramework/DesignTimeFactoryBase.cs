using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Blocks.EntityFrameworkCore;

public abstract class DesignTimeFactoryBase<TContext> : IDesignTimeDbContextFactory<TContext>
    where TContext : DbContext
{
    public TContext CreateDbContext(string[] args)
    {
        var config = Host.CreateApplicationBuilder(args).Configuration;
        var connectionString = config.GetConnectionString("Database")
            ?? throw new InvalidOperationException("Missing 'ConnectionStrings:Database'.");

        var builder = new DbContextOptionsBuilder<TContext>();

        ConfigureProvider(builder, connectionString);
        //builder.UseSqlServer(cs);

        return CreateContext(builder.Options);
    }

    protected abstract void ConfigureProvider(DbContextOptionsBuilder<TContext> b, string cs);

    protected virtual TContext CreateContext(DbContextOptions<TContext> options)
        => Activator.CreateInstance(typeof(TContext), options) as TContext
           ?? throw new InvalidOperationException($"{typeof(TContext).Name} needs ctor(DbContextOptions<{typeof(TContext).Name}>)");
}
