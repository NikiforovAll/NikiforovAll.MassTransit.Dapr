namespace MassTransit.PubSubIntegration;

using MassTransit.Configuration;
using MassTransit.Middleware;

public class PubSubProcessorContext :
    BasePipeContext,
    ProcessorContext
{
    private readonly IHostConfiguration hostConfiguration;

    public PubSubProcessorContext(
        IHostConfiguration hostConfiguration,
        IReceiveSettings receiveSettings,
        CancellationToken cancellationToken)
        : base(cancellationToken)
    {
        this.hostConfiguration = hostConfiguration;
        this.ReceiveSettings = receiveSettings;
    }
    public IReceiveSettings ReceiveSettings { get; }
}
