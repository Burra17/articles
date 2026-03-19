using Blocks.Core.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blocks.EntityFrameworkCore;

public static class DbContextExtensions
{
    private static readonly JsonSerializerSettings DefaultSettings = new()
    {
        ContractResolver = new PrivateContractResolver(),
        Converters = { new StringEnumConverter() },
        TypeNameHandling = TypeNameHandling.Auto
    };

    public static IHost Migrate<TDbContext>(this IHost app)
        where TDbContext : DbContext
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TDbContext>();
        context.Database.Migrate();

        return app;
    }

    public static void SeedTestData<TContext>(this IServiceProvider services, Action<TContext> seeder)
        where TContext : DbContext
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TContext>();

        using var transaction = context.Database.BeginTransaction();
        seeder(context);
        transaction.Commit();
    }

    public static void SeedFromJsonFile<TEntity>(this DbContext context, string folderPath = "Data/Test")
        where TEntity : class
    {
        if (context.Set<TEntity>().Any())
            return;

        var filePath = $"{AppContext.BaseDirectory}{folderPath}/{typeof(TEntity).Name}.json";
        if (!File.Exists(filePath))
            return;

        var collection = JsonConvert.DeserializeObject<TEntity[]>(File.ReadAllText(filePath), DefaultSettings);
        if (collection is { Length: > 0 })
            context.Set<TEntity>().AddRange(collection);

        context.SaveChanges();
    }
}
