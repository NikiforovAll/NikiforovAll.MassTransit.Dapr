namespace MassTransit.PubSubIntegration.Configuration;

using Transports;

public interface IPubSubHostConfiguration :
    ISpecification
{
    IConnectionContextSupervisor ConnectionContextSupervisor { get; }

    IPubSubReceiveEndpointSpecification CreateSpecification(
        string pubSubName,
        string topic,
        Action<IPubSubReceiveEndpointConfigurator> configure);

    IPubSubRider Build(IRiderRegistrationContext context, IBusInstance busInstance);
}
