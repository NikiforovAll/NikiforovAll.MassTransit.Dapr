namespace MassTransit.PubSubIntegration.Configuration;

using Transports;

public interface IPubSubProducerSpecification :
    ISpecification
{
    IPubSubProducerProvider CreateProducerProvider(IBusInstance busInstance);
}
