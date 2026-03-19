using Blocks.Core.Cache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Submission.Domain.Entities;

namespace Submission.Persistence;

public class SubmissionDbContext(DbContextOptions<SubmissionDbContext> options, IMemoryCache cache) : DbContext(options)
{
    #region Entities
    public virtual DbSet<Article> Articles { get; set; }
    public virtual DbSet<Journal> Journals { get; set; }

    public virtual DbSet<Person> People { get; set; }
    public virtual DbSet<Author> Authors { get; set; }
    public virtual DbSet<ArticleActor> ArticleActors { get; set; }

    public virtual DbSet<Asset> Assets { get; set; }
    public virtual DbSet<AssetTypeDefinition> AssetTypes { get; set; }

    public virtual DbSet<ArticleStageTransition> ArticleStageTransitions { get; set; }
    #endregion

    public virtual IEnumerable<TEntity> GetAllCached<TEntity>()
        where TEntity : class, ICacheable
        => cache.GetOrCreateByType(entry => this.Set<TEntity>().AsNoTracking().ToList());

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
    }
}
