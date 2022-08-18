namespace MassTransit.PubSubIntegration;

using MassTransit.Middleware;

public class SharedProducerContext :
    ProxyPipeContext,
    ProducerContext
{
    private readonly ProducerContext context;

    public SharedProducerContext(ProducerContext context, CancellationToken cancellationToken)
        : base(context)
    {
        this.context = context;
        this.CancellationToken = cancellationToken;
    }

    public override CancellationToken CancellationToken { get; }

    public ISerialization Serializer => this.context.Serializer;

    public ValueTask DisposeAsync()
    {
        return this.context.DisposeAsync();
    }
}
