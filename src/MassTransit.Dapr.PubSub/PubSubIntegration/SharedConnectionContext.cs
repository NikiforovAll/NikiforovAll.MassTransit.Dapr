namespace MassTransit.PubSubIntegration;

using MassTransit.Middleware;

public class SharedConnectionContext :
    ProxyPipeContext,
    ConnectionContext
{
    private readonly ConnectionContext context;

    public SharedConnectionContext(ConnectionContext context, CancellationToken cancellationToken)
        : base(context)
    {
        this.context = context;
        this.CancellationToken = cancellationToken;
    }

    public override CancellationToken CancellationToken { get; }
}
