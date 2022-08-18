namespace MassTransit.PubSubIntegration;

using Initializers;

public class PubSubProducer :
    IPubSubProducer
{
    private readonly PubSubSendTransportContext context;

    public PubSubProducer(PubSubSendTransportContext context)
    {
        this.context = context;
    }

    public Task Produce<T>(T message, CancellationToken cancellationToken = default)
        where T : class
    {
        return this.Produce(message, Pipe.Empty<PubSubSendContext<T>>(), cancellationToken);
    }

    public Task Produce<T>(IEnumerable<T> messages, CancellationToken cancellationToken = default)
        where T : class
    {
        return this.Produce(messages, Pipe.Empty<PubSubSendContext<T>>(), cancellationToken);
    }

    public Task Produce<T>(T message, IPipe<PubSubSendContext<T>> pipe, CancellationToken cancellationToken)
        where T : class
    {
        return this.context.Send(new SendPipe<T>(message, this.context, pipe, cancellationToken), cancellationToken);
    }

    public async Task Produce<T>(
        IEnumerable<T> messages, IPipe<PubSubSendContext<T>> pipe, CancellationToken cancellationToken = default)
        where T : class
    {
        foreach (var message in messages)
        {
            await this.context.Send(new SendPipe<T>(message, this.context, pipe, cancellationToken), cancellationToken);
        }
    }

    public Task Produce<T>(object values, CancellationToken cancellationToken = default)
        where T : class
    {
        return this.Produce(values, Pipe.Empty<PubSubSendContext<T>>(), cancellationToken);
    }

    public Task Produce<T>(IEnumerable<object> values, CancellationToken cancellationToken = default)
        where T : class
    {
        return this.Produce(values, Pipe.Empty<PubSubSendContext<T>>(), cancellationToken);
    }

    public async Task Produce<T>(object values, IPipe<PubSubSendContext<T>> pipe, CancellationToken cancellationToken = default)
        where T : class
    {
        (var message, var sendPipe) = await MessageInitializerCache<T>.InitializeMessage(values, cancellationToken);

        await this.context.Send(new SendPipe<T>(message, this.context, pipe, cancellationToken, sendPipe), cancellationToken).ConfigureAwait(false);
    }

    public Task Produce<T>(
        IEnumerable<object> values, IPipe<PubSubSendContext<T>> pipe, CancellationToken cancellationToken = default)
        where T : class
    {
        // TODO change this to use the proper header initialization, etc.

        var messageTasks = values.Select(value => MessageInitializerCache<T>.Initialize(value, cancellationToken)).ToArray();

        async Task ProduceAsync()
        {
            var contexts = await Task.WhenAll(messageTasks).ConfigureAwait(false);

            await this.Produce(contexts.Select(x => x.Message), pipe, cancellationToken).ConfigureAwait(false);
        }

        return ProduceAsync();
    }

    public ConnectHandle ConnectSendObserver(ISendObserver observer)
    {
        return this.context.ConnectSendObserver(observer);
    }

    private sealed class SendPipe<T> :
        IPipe<ProducerContext>
        where T : class
    {
        private readonly CancellationToken cancellationToken;
        private readonly PubSubSendTransportContext context;
        private readonly T message;
        private readonly IPipe<PubSubSendContext<T>> pipe;
        private readonly IPipe<SendContext<T>> sendPipe;

        public SendPipe(T message, PubSubSendTransportContext context, IPipe<PubSubSendContext<T>> pipe, CancellationToken cancellationToken,
            IPipe<SendContext<T>> sendPipe = null)
        {
            this.message = message;
            this.context = context;
            this.pipe = pipe;
            this.cancellationToken = cancellationToken;
            this.sendPipe = sendPipe;
        }

        public void Probe(ProbeContext context)
        {
        }

        public Task Send(ProducerContext context) => throw new NotImplementedException();
    }
}
