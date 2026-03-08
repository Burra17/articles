using Blocks.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Blocks.EntityFrameworkCore;

public interface IRepository<TEntity>
        where TEntity : class, IEntity
{
    Task <TEntity?> FindByIdAsync(int id);
    Task <TEntity> GetByIdAsync(int id);
    Task <TEntity> AddAsync(TEntity entity);
    TEntity Update(TEntity entity);
    void Remove(TEntity entity);

    Task <bool> DeleteByIdAsync(int id);

}

public class Repository<TContext, TEntity>
    where TContext : DbContext
    where TEntity : class, IEntity
{
    protected readonly TContext _dbContext;
    protected readonly DbSet<TEntity> _entity;
    public Repository(TContext dbContext)
    {
        _dbContext = dbContext;
        _entity = _dbContext.Set<TEntity>();
    }

    public TContext Context => _dbContext;
    public virtual DbSet<TEntity> Entity => _entity;
    public string TableName => _dbContext.Model.FindEntityType(typeof(TEntity))?.GetTableName()!;

    protected virtual IQueryable<TEntity> Query() => _entity;
    public virtual async Task<TEntity?> FindByIdAsync(int id)
        => await _entity.FindAsync(id);

    public virtual async Task<TEntity?> GetByIdAsync(int id)
        => await Query().FirstOrDefaultAsync(e => e.Id.Equals(id));

    public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken ct = default)
        => (await _entity.AddAsync(entity, ct)).Entity;

    public virtual TEntity Update(TEntity entity)
        => _entity.Update(entity).Entity;

    public virtual void Remove(TEntity entity)
        => _entity.Remove(entity);

    public virtual async Task<bool> DeleteByIdAsync(int id)
    {
        var rowsAffected = await _dbContext.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM {TableName} WHERE Id = {id}");
        return rowsAffected > 0;
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
         => await _dbContext.SaveChangesAsync(ct);
}
