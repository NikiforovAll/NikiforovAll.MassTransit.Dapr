namespace MassTransit.PubSubIntegration;

using MassTransit.Middleware;

public class PubSubProducerContext :
    BasePipeContext,
    ProducerContext
{
    public PubSubProducerContext(ISerialization serializers, CancellationToken cancellationToken)
        : base(cancellationToken)
    {
        this.Serializer = serializers;
    }

    public ISerialization Serializer { get; }

    public ValueTask DisposeAsync()
    {
        return default;
    }
}
