using Microsoft.Extensions.Caching.Distributed;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Backplane;
using ZiggyCreatures.Caching.Fusion.Events;
using ZiggyCreatures.Caching.Fusion.Plugins;
using ZiggyCreatures.Caching.Fusion.Serialization;

public class FakeFusionCache : IFusionCache
{
    public string CacheName => throw new NotImplementedException();

    public string InstanceId => throw new NotImplementedException();

    public FusionCacheEntryOptions DefaultEntryOptions { get; } = new FusionCacheEntryOptions();

    public FusionCacheEntryOptionsProvider? DefaultEntryOptionsProvider { get; } = null;

    public bool HasDistributedCache => throw new NotImplementedException();

    public IDistributedCache? DistributedCache => throw new NotImplementedException();

    public bool HasBackplane => throw new NotImplementedException();

    public IFusionCacheBackplane? Backplane => throw new NotImplementedException();

    public FusionCacheEventsHub Events => throw new NotImplementedException();

    public ValueTask<TValue> GetOrSetAsync<TValue>(
  string key,
  Func<FusionCacheFactoryExecutionContext<TValue>, CancellationToken, Task<TValue>> factory,
  MaybeValue<TValue> failSafeDefaultValue = default,
  FusionCacheEntryOptions? options = null,
  IEnumerable<string>? tags = null,
  CancellationToken token = default)
    {
        return new ValueTask<TValue>(factory(null!, token));
    }
    public Task<TValue?> GetAsync<TValue>(string key, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
    public Task SetAsync<TValue>(string key, TValue value, TimeSpan duration, IEnumerable<string>? tags = null, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
    public Task RemoveAsync(string key, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
    public Task<bool> ContainsKeyAsync(string key, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
    public Task ClearAsync(CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
    public Task<IEnumerable<string>> GetKeysAsync(CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }
    public Task<IEnumerable<string>> GetKeysByTagAsync(string tag, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
    public Task<IEnumerable<string>> GetTagsAsync(string key, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
    public Task AddTagAsync(string key, string tag, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
    public Task RemoveTagAsync(string key, string tag, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
    public Task<IEnumerable<string>> GetAllTagsAsync(CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
    public Task<bool> TryGetValueAsync<TValue>(string key, out TValue? value, CancellationToken token = default)
    {
        value = default;
        return Task.FromResult(false);
    }
    public Task<bool> TryGetValueAsync<TValue>(string key, Func<CancellationToken, Task<TValue>> factory, out TValue? value, TimeSpan duration, CancellationToken token = default)
    {
        value = default;
        return Task.FromResult(false);
    }
    public Task<bool> TryGetValueAsync<TValue>(string key, Func<CancellationToken, Task<TValue>> factory, out TValue? value, TimeSpan duration, IEnumerable<string>? tags = null, CancellationToken token = default)
    {
        value = default;
        return Task.FromResult(false);
    }

    public FusionCacheEntryOptions CreateEntryOptions(Action<FusionCacheEntryOptions>? setupAction = null, TimeSpan? duration = null)
    {
        throw new NotImplementedException();
    }

    public TValue GetOrSet<TValue>(string key, Func<FusionCacheFactoryExecutionContext<TValue>, CancellationToken, TValue> factory, MaybeValue<TValue> failSafeDefaultValue = default, FusionCacheEntryOptions? options = null, IEnumerable<string>? tags = null, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask<TValue> GetOrSetAsync<TValue>(string key, TValue defaultValue, FusionCacheEntryOptions? options = null, IEnumerable<string>? tags = null, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public TValue GetOrSet<TValue>(string key, TValue defaultValue, FusionCacheEntryOptions? options = null, IEnumerable<string>? tags = null, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask<TValue?> GetOrDefaultAsync<TValue>(string key, TValue? defaultValue = default, FusionCacheEntryOptions? options = null, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public TValue? GetOrDefault<TValue>(string key, TValue? defaultValue = default, FusionCacheEntryOptions? options = null, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask<MaybeValue<TValue>> TryGetAsync<TValue>(string key, FusionCacheEntryOptions? options = null, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public MaybeValue<TValue> TryGet<TValue>(string key, FusionCacheEntryOptions? options = null, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask SetAsync<TValue>(string key, TValue value, FusionCacheEntryOptions? options = null, IEnumerable<string>? tags = null, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public void Set<TValue>(string key, TValue value, FusionCacheEntryOptions? options = null, IEnumerable<string>? tags = null, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask RemoveAsync(string key, FusionCacheEntryOptions? options = null, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public void Remove(string key, FusionCacheEntryOptions? options = null, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask ExpireAsync(string key, FusionCacheEntryOptions? options = null, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public void Expire(string key, FusionCacheEntryOptions? options = null, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask RemoveByTagAsync(string tag, FusionCacheEntryOptions? options = null, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask RemoveByTagAsync(IEnumerable<string> tags, FusionCacheEntryOptions? options = null, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public void RemoveByTag(string tag, FusionCacheEntryOptions? options = null, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public void RemoveByTag(IEnumerable<string> tags, FusionCacheEntryOptions? options = null, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask ClearAsync(bool allowFailSafe = true, FusionCacheEntryOptions? options = null, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public void Clear(bool allowFailSafe = true, FusionCacheEntryOptions? options = null, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public IFusionCache SetupSerializer(IFusionCacheSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public IFusionCache SetupDistributedCache(IDistributedCache distributedCache)
    {
        throw new NotImplementedException();
    }

    public IFusionCache SetupDistributedCache(IDistributedCache distributedCache, IFusionCacheSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public IFusionCache RemoveDistributedCache()
    {
        throw new NotImplementedException();
    }

    public IFusionCache SetupBackplane(IFusionCacheBackplane backplane)
    {
        throw new NotImplementedException();
    }

    public IFusionCache RemoveBackplane()
    {
        throw new NotImplementedException();
    }

    public void AddPlugin(IFusionCachePlugin plugin)
    {
        throw new NotImplementedException();
    }

    public bool RemovePlugin(IFusionCachePlugin plugin)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}