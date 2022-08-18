namespace MassTransit.PubSubIntegration;

public class CachedPubSubProducerProvider :
    IPubSubProducerProvider
{
    private readonly IPubSubProducerCache<Uri> cache;
    private readonly IPubSubProducerProvider provider;

    public CachedPubSubProducerProvider(IPubSubProducerProvider provider)
    {
        this.provider = provider;
        this.cache = new PubSubProducerCache<Uri>();
    }

    public Task<IPubSubProducer> GetProducer(Uri address)
    {
        return this.cache.GetProducer(address, this.provider.GetProducer);
    }
}
