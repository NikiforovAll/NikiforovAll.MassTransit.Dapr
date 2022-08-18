namespace MassTransit;

public interface IPubSubEndpointConnector
{
    HostReceiveEndpointHandle ConnectPubSubEndpoint(
        string pubSubName,
        string topic,
        Action<IRiderRegistrationContext, IPubSubReceiveEndpointConfigurator> configure);
}
