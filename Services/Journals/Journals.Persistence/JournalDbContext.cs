using Redis.OM;
using Redis.OM.Searching;
using StackExchange.Redis;
using Submission.Domain.Entities;

namespace Journals.Persistence;

public class JournalDbContext
{
    private readonly RedisConnectionProvider _provider;
    private readonly IDatabase _redisDb;

    public JournalDbContext(IConnectionMultiplexer redis, RedisConnectionProvider provider)
        => (_redisDb, _provider) = (redis.GetDatabase(), provider);

    public IRedisCollection<Journal> Journals => _provider.RedisCollection<Journal>();
    public IRedisCollection<Journal> Editors => _provider.RedisCollection<Journal>();

    public RedisConnectionProvider Provider => _provider;
}
