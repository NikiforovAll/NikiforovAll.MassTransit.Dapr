namespace MassTransit.PubSubIntegration.Middleware;

using Transports;

public class PubSubConsumerFilter :
    IFilter<ProcessorContext>
{
    private readonly ReceiveEndpointContext context;

    public PubSubConsumerFilter(ReceiveEndpointContext context, PubSubDataReceiver receiver)
    {
        this.context = context;
        this.Receiver = receiver;
    }

    public IPubSubDataReceiver Receiver { get; private set; }

    public async Task Send(ProcessorContext context, IPipe<ProcessorContext> next)
    {

        var receiver = this.Receiver;

        await receiver.Start().ConfigureAwait(false);

        await receiver.Ready.ConfigureAwait(false);

        this.context.AddConsumeAgent(receiver);

        await this.context.TransportObservers.NotifyReady(this.context.InputAddress).ConfigureAwait(false);

        try
        {
            await receiver.Completed.ConfigureAwait(false);
        }
        finally
        {
            DeliveryMetrics metrics = receiver;

            await this.context.TransportObservers.NotifyCompleted(this.context.InputAddress, metrics).ConfigureAwait(false);

            this.context.LogConsumerCompleted(metrics.DeliveryCount, metrics.ConcurrentDeliveryCount);
        }

        await next.Send(context).ConfigureAwait(false);
    }

    public void Probe(ProbeContext context)
    {
    }
}
