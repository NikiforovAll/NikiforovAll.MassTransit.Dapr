namespace MassTransit.PubSubIntegration;

using MassTransit.Middleware;

public class PubSubConnectionContext :
    BasePipeContext,
    ConnectionContext
{
    public PubSubConnectionContext(
        CancellationToken cancellationToken)
        : base(cancellationToken)
    {
    }
}
