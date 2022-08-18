namespace MassTransit;

public static class PubSubEndpointConnectorExtensions
{
    /// <summary>
    /// Connect to PubSub using default consumer group.
    /// </summary>
    /// <param name="connector"></param>
    /// <param name="pubSubName">Event Hub name</param>
    /// <param name="configure"></param>
    public static HostReceiveEndpointHandle ConnectPubSubEndpoint(
        this IPubSubEndpointConnector connector,
        string pubSubName,
        string topic,
        Action<IRiderRegistrationContext, IPubSubReceiveEndpointConfigurator> configure)
    {
        return connector.ConnectPubSubEndpoint(pubSubName, topic, configure);
    }
}
