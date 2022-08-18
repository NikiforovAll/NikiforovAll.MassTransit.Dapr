namespace MassTransit.PubSubIntegration;

using Transports;

public sealed class PubSubReceiveContext :
    BaseReceiveContext,
    PubSubConsumeContext,
    ReceiveLockContext
{
    private readonly PubSubEventArgs eventArgs;

    public PubSubReceiveContext(PubSubEventArgs eventArgs, ReceiveEndpointContext receiveEndpointContext)
        : base(false, receiveEndpointContext)
    {
        this.Body = eventArgs.Body is null
        ? new EmptyMessageBody()
        : new StringMessageBody(eventArgs.Body);
        this.eventArgs = eventArgs;
    }

    protected override IHeaderProvider HeaderProvider => new DictionaryHeaderProvider(
        this.eventArgs.Headers);

    public override MessageBody Body { get; }

    public IDictionary<string, object> Properties => default;
    public IReadOnlyDictionary<string, object> SystemProperties => default;

    public Task Complete()
    {
        return Task.CompletedTask;
    }

    public Task Faulted(Exception exception)
    {
        return Task.CompletedTask;
    }

    public Task ValidateLockStatus()
    {
        return Task.CompletedTask;
    }
}
