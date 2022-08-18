namespace MassTransit.PubSubIntegration;

using Transports;

public class ConsumeContextPubSubProducerProvider :
    IPubSubProducerProvider
{
    private readonly ConsumeContext consumeContext;
    private readonly IPubSubProducerProvider provider;

    public ConsumeContextPubSubProducerProvider(IPubSubProducerProvider provider, ConsumeContext consumeContext)
    {
        this.provider = provider;
        this.consumeContext = consumeContext;
    }

    public async Task<IPubSubProducer> GetProducer(Uri address)
    {
        var producer = await this.provider.GetProducer(address).ConfigureAwait(false);
        return new Producer(producer, this.consumeContext);
    }

    private sealed class Producer :
        IPubSubProducer
    {
        private readonly ConsumeContext consumeContext;
        private readonly IPubSubProducer producer;

        public Producer(IPubSubProducer producer, ConsumeContext consumeContext)
        {
            this.producer = producer;
            this.consumeContext = consumeContext;
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return this.producer.ConnectSendObserver(observer);
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

        public Task Produce<T>(T message, IPipe<PubSubSendContext<T>> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            var sendPipeAdapter = new ConsumeSendPipeAdapter<T>(pipe, this.consumeContext);
            return this.producer.Produce(message, sendPipeAdapter, cancellationToken);
        }

        public Task Produce<T>(IEnumerable<T> messages, IPipe<PubSubSendContext<T>> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            var sendPipeAdapter = new ConsumeSendPipeAdapter<T>(pipe, this.consumeContext);

            return this.producer.Produce(messages, sendPipeAdapter, cancellationToken);
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

        public Task Produce<T>(object values, IPipe<PubSubSendContext<T>> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            var sendPipeAdapter = new ConsumeSendPipeAdapter<T>(pipe, this.consumeContext);

            return this.producer.Produce(values, sendPipeAdapter, cancellationToken);
        }

        public Task Produce<T>(IEnumerable<object> values, IPipe<PubSubSendContext<T>> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            var sendPipeAdapter = new ConsumeSendPipeAdapter<T>(pipe, this.consumeContext);

            return this.producer.Produce(values, sendPipeAdapter, cancellationToken);
        }
    }

    private sealed class ConsumeSendPipeAdapter<T> :
        IPipe<PubSubSendContext<T>>,
        ISendPipe
        where T : class
    {
        private readonly ConsumeContext consumeContext;
        private readonly IPipe<PubSubSendContext<T>> pipe;

        public ConsumeSendPipeAdapter(IPipe<PubSubSendContext<T>> pipe, ConsumeContext consumeContext)
        {
            this.pipe = pipe;
            this.consumeContext = consumeContext;
        }

        public async Task Send(PubSubSendContext<T> context)
        {
            if (this.consumeContext != null)
            {
                context.TransferConsumeContextHeaders(this.consumeContext);
            }

            if (this.pipe.IsNotEmpty())
            {
                await this.pipe.Send(context).ConfigureAwait(false);
            }
        }

        public void Probe(ProbeContext context)
        {
            this.pipe.Probe(context);
        }

        public Task Send<TMessage>(SendContext<TMessage> context)
            where TMessage : class
        {
            return Task.CompletedTask;
        }
    }
}
