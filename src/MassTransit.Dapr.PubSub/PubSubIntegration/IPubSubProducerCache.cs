namespace MassTransit.PubSubIntegration;

public interface IPubSubProducerCache<TKey>
{
    Task<IPubSubProducer> GetProducer(TKey key, Func<TKey, Task<IPubSubProducer>> factory);
}
