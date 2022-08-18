namespace MassTransit.PubSubIntegration;

using Internals.Caching;
using Transports;

public class PubSubProducerCache<TKey> :
    IPubSubProducerCache<TKey>
{
    private readonly ICache<TKey, CachedPubSubProducer<TKey>, ITimeToLiveCacheValue<CachedPubSubProducer<TKey>>> cache;

    public PubSubProducerCache()
    {
        var options = new CacheOptions { Capacity = SendEndpointCacheDefaults.Capacity };
        var policy = new TimeToLiveCachePolicy<CachedPubSubProducer<TKey>>(SendEndpointCacheDefaults.MaxAge);

        this.cache = new MassTransitCache<TKey, CachedPubSubProducer<TKey>, ITimeToLiveCacheValue<CachedPubSubProducer<TKey>>>(policy, options);
    }

    public async Task<IPubSubProducer> GetProducer(TKey key, Func<TKey, Task<IPubSubProducer>> factory)
    {
        return await this.cache.GetOrAdd(key, x => GetProducerFromFactory(x, factory)).ConfigureAwait(false);
    }

    private static async Task<CachedPubSubProducer<TKey>> GetProducerFromFactory(TKey address, Func<TKey, Task<IPubSubProducer>> factory)
    {
        var sendEndpoint = await factory(address).ConfigureAwait(false);

        return new CachedPubSubProducer<TKey>(address, sendEndpoint);
    }
}
