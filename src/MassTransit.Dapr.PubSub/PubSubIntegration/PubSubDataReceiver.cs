namespace MassTransit.PubSubIntegration;

using Internals;
using MassTransit.Middleware;
using Microsoft.AspNetCore.Http;
using Transports;
using Util;


public class PubSubDataReceiver :
    Agent,
    IPubSubDataReceiver, IAsyncDisposable
{
    private readonly ReceiveEndpointContext context;
    private readonly TaskCompletionSource<bool> deliveryComplete;
    private readonly IReceivePipeDispatcher dispatcher;
    private readonly ChannelExecutor executor;

    public PubSubDataReceiver(ReceiveEndpointContext context, int concurrentMessageLimit)
    {
        this.context = context;

        this.deliveryComplete = TaskUtil.GetTask<bool>();

        this.dispatcher = context.CreateReceivePipeDispatcher();
        this.dispatcher.ZeroActivity += this.HandleDeliveryComplete;


        this.executor = new ChannelExecutor(
            concurrentMessageLimit,
            concurrentMessageLimit);
    }

    public long DeliveryCount => this.dispatcher.DispatchCount;

    public int ConcurrentDeliveryCount => this.dispatcher.MaxConcurrentDispatchCount;

    public async Task Start()
    {
        this.SetReady();
    }

    public async Task EnqueueMessage(PubSubEventArgs eventArgs)
    {
        LogContext.SetCurrentIfNull(this.context.LogContext);

        try
        {
            await this.executor.Push(() => this.Handle(eventArgs), this.Stopping).ConfigureAwait(false);
        }
        catch (OperationCanceledException e) when (e.CancellationToken == this.Stopping)
        {
        }
    }

    private async Task Handle(PubSubEventArgs eventArgs)
    {
        var context = new PubSubReceiveContext(eventArgs, this.context);

        try
        {
            await this.dispatcher.Dispatch(context, context).ConfigureAwait(false);
        }
        finally
        {
            context.Dispose();
        }
    }

    private async Task HandleDeliveryComplete()
    {
        if (this.IsStopping)
        {
            LogContext.Debug?.Log("Consumer shutdown completed: {InputAddress}", this.context.InputAddress);

            this.deliveryComplete.TrySetResult(true);
        }
    }

    protected override async Task StopAgent(StopContext context)
    {
        await this.executor.DisposeAsync().ConfigureAwait(false);

        LogContext.Debug?.Log("Stopping consumer: {InputAddress}", this.context.InputAddress);

        this.SetCompleted(this.ActiveAndActualAgentsCompleted(context));

        await this.Completed.ConfigureAwait(false);
    }

    private async Task ActiveAndActualAgentsCompleted(StopContext context)
    {
        if (this.dispatcher.ActiveDispatchCount > 0)
        {
            try
            {
                await this.deliveryComplete.Task.OrCanceled(context.CancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                LogContext.Warning?.Log("Stop canceled waiting for message consumers to complete: {InputAddress}", this.context.InputAddress);
            }
        }
    }

    public ValueTask DisposeAsync() => this.executor.DisposeAsync();
}
