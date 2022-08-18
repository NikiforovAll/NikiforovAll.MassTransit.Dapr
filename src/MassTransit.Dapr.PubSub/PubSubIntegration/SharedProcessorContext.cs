namespace MassTransit.PubSubIntegration;

using MassTransit.Middleware;

public class SharedProcessorContext :
    ProxyPipeContext,
    ProcessorContext
{
    private readonly ProcessorContext context;

    public SharedProcessorContext(ProcessorContext context, CancellationToken cancellationToken)
        : base(context)
    {
        this.context = context;
        this.CancellationToken = cancellationToken;
    }

    public override CancellationToken CancellationToken { get; }

    public IReceiveSettings ReceiveSettings => this.context.ReceiveSettings;
}
