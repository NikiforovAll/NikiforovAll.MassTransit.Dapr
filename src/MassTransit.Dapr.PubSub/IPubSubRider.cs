namespace MassTransit;

using MassTransit.PubSubIntegration.Configuration;
using Transports;

public interface IPubSubRider :
    IRiderControl,
    IPubSubEndpointConnector
{
    IPubSubProducerProvider GetProducerProvider(ConsumeContext consumeContext = default);

    IList<IPubSubReceiveEndpointSpecification> EndpointSpecs { get; }
}
