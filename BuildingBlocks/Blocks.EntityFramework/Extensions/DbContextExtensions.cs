using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Blocks.EntityFrameworkCore;

public static class DbContextExtensions
{
    public static IHost Migrate<TDbContext>(this IHost app)
        where TDbContext : DbContext
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TDbContext>();
        context.Database.Migrate();

        return app;
    }
}
