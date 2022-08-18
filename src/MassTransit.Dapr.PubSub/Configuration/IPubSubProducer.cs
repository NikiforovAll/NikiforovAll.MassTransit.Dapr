namespace MassTransit;

public interface IPubSubProducerConfigurator :
    ISendObserverConnector,
    ISendPipelineConfigurator
{
    /// <summary>
    /// Sets the outbound message serializer
    /// </summary>
    /// <param name="factory">The factory to create the message serializer</param>
    /// <param name="isSerializer"></param>
    void AddSerializer(ISerializerFactory factory, bool isSerializer = true);
}
