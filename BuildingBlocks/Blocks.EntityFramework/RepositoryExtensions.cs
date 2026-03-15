using Blocks.Core;
using Blocks.Domain.Entities;
using Blocks.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Blocks.EntityFrameworkCore;

public static class RepositoryExtensions
{
    public static async Task<TEntity> FindByIdOrThrowAsync<TEntity, TContext>(this RepositoryBase<TContext, TEntity> repository, int id)
            where TContext : DbContext
            where TEntity : class, IEntity
    {
        var entity = await repository.FindByIdAsync(id);
        if (entity == null)
            throw new NotFoundException($"{typeof(TEntity).Name} not found.");

        return entity;
    }

    public static async Task<TEntity> FindByIdOrThrowAsync<TEntity>(this DbSet<TEntity> dbSet, int id)
        where TEntity : class, IEntity
    {
        var entity = await dbSet.FindByIdOrThrowAsync(id);
        if (entity == null)
            throw new NotFoundException($"{typeof(TEntity).Name} not found.");

        return entity!;
    }

    public static async Task<TEntity> GetByIdOrThrowAsync<TEntity, TContext> (this RepositoryBase<TContext, TEntity> repository, int id, CancellationToken ct = default)
            where TContext : DbContext
            where TEntity : class, IEntity
    {
        var entity = await repository.GetByIdAsync(id, ct);
        if (entity == null)
            throw new NotFoundException($"{typeof(TEntity).Name} not found.");
        return entity!;
    }

    public static async Task<TEntity> SingleOrThrowAsync<TEntity>(this IQueryable<TEntity> source, Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
        where TEntity : class, IEntity<int>
        => Guard.NotFound(await source.SingleOrDefaultAsync(predicate, ct));
}
