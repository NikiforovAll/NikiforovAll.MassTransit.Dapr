namespace MassTransit.PubSubIntegration;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Caching;


public class CachedPubSubProducer<TKey> :
    IPubSubProducer,
    INotifyValueUsed,
    IAsyncDisposable
{
    private readonly IPubSubProducer producer;

    public CachedPubSubProducer(TKey key, IPubSubProducer producer)
    {
        this.Key = key;
        this.producer = producer;
    }

    public TKey Key { get; }

    public async ValueTask DisposeAsync()
    {
        switch (this.producer)
        {
            case IAsyncDisposable disposable:
                await disposable.DisposeAsync().ConfigureAwait(false);
                break;

            case IDisposable disposable:
                disposable.Dispose();
                break;
        }
    }

    public ConnectHandle ConnectSendObserver(ISendObserver observer)
    {
        Used?.Invoke();
        return this.producer.ConnectSendObserver(observer);
    }

    public Task Produce<T>(T message, CancellationToken cancellationToken = default)
        where T : class
    {
        Used?.Invoke();
        return this.producer.Produce(message, cancellationToken);
    }

    public Task Produce<T>(IEnumerable<T> messages, CancellationToken cancellationToken = default)
        where T : class
    {
        Used?.Invoke();
        return this.producer.Produce(messages, cancellationToken);
    }

    public Task Produce<T>(T message, IPipe<PubSubSendContext<T>> pipe, CancellationToken cancellationToken = default)
        where T : class
    {
        Used?.Invoke();
        return this.producer.Produce(message, pipe, cancellationToken);
    }

    public Task Produce<T>(IEnumerable<T> messages, IPipe<PubSubSendContext<T>> pipe, CancellationToken cancellationToken = default)
        where T : class
    {
        Used?.Invoke();
        return this.producer.Produce(messages, pipe, cancellationToken);
    }

    public Task Produce<T>(object values, CancellationToken cancellationToken = default)
        where T : class
    {
        Used?.Invoke();
        return this.producer.Produce<T>(values, cancellationToken);
    }

    public Task Produce<T>(IEnumerable<object> values, CancellationToken cancellationToken = default)
        where T : class
    {
        Used?.Invoke();
        return this.producer.Produce<T>(values, cancellationToken);
    }

    public Task Produce<T>(object values, IPipe<PubSubSendContext<T>> pipe, CancellationToken cancellationToken = default)
        where T : class
    {
        Used?.Invoke();
        return this.producer.Produce(values, pipe, cancellationToken);
    }

    public Task Produce<T>(IEnumerable<object> values, IPipe<PubSubSendContext<T>> pipe, CancellationToken cancellationToken = default)
        where T : class
    {
        Used?.Invoke();
        return this.producer.Produce(values, pipe, cancellationToken);
    }

    public event Action Used;
}
