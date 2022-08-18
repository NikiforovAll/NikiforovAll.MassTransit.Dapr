namespace MassTransit;

public interface IPubSubFactoryConfigurator :
    IRiderFactoryConfigurator,
    ISendObserverConnector,
    ISendPipelineConfigurator
{
    /// <summary>
    /// Subscribe to PubSub messages
    /// </summary>
    /// <param name="pubSubName">PubSub Name</param>
    /// <param name="topic">Consumer Group</param>
    /// <param name="configure"></param>
    void ReceiveEndpoint(string pubSubName, string topic, Action<IPubSubReceiveEndpointConfigurator> configure);

    /// <summary>
    /// Sets the outbound message serializer
    /// </summary>
    /// <param name="factory"></param>
    /// <param name="isSerializer"></param>
    void AddSerializer(ISerializerFactory factory, bool isSerializer = true);
}
