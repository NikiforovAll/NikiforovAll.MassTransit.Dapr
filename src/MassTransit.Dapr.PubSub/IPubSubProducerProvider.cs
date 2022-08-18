namespace MassTransit;

public interface IPubSubProducerProvider
{
    Task<IPubSubProducer> GetProducer(Uri address);
}
