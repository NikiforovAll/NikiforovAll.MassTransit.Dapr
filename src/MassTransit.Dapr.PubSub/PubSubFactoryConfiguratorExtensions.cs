namespace MassTransit;

public static class PubSubFactoryConfiguratorExtensions
{
    /// <summary>
    /// Subscribe to PubSub messages using default consumer group
    /// </summary>
    /// <param name="configurator"></param>
    /// <param name="pubSubName">Event Hub name</param>
    /// <param name="configure"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void Subscribe(
        this IPubSubFactoryConfigurator configurator,
        string pubSubName,
        string topic,
        Action<IPubSubReceiveEndpointConfigurator> configure)
    {
        if (configurator == null)
        {
            throw new ArgumentNullException(nameof(configurator));
        }

        configurator.ReceiveEndpoint(pubSubName, topic, configure);
    }
}
