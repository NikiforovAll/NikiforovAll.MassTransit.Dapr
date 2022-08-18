namespace MassTransit.PubSubIntegration;

public interface ProducerContext :
    PipeContext,
    IAsyncDisposable
{
    ISerialization Serializer { get; }
}
