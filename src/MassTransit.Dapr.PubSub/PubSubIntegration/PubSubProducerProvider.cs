namespace MassTransit.PubSubIntegration;

using Configuration;
using Observables;
using Transports;


public class PubSubProducerProvider :
    IPubSubProducerProvider
{
    private readonly IBusInstance busInstance;
    private readonly IPubSubHostConfiguration hostConfiguration;
    private readonly SendObservable sendObservable;
    private readonly ISendPipe sendPipe;
    private readonly ISerialization serializers;

    public PubSubProducerProvider(IPubSubHostConfiguration hostConfiguration, IBusInstance busInstance, ISendPipe sendPipe,
        SendObservable sendObservable, ISerialization serializers)
    {
        this.hostConfiguration = hostConfiguration;
        this.busInstance = busInstance;
        this.sendPipe = sendPipe;
        this.sendObservable = sendObservable;
        this.serializers = serializers;
    }

    public Task<IPubSubProducer> GetProducer(Uri address)
    {
        var context = new PubSubProducerSendTransportContext(this.hostConfiguration, this.sendPipe, this.busInstance.HostConfiguration, address, this.serializers);

        if (this.sendObservable.Count > 0)
        {
            context.ConnectSendObserver(this.sendObservable);
        }

        IPubSubProducer PubSubProducer = new PubSubProducer(context);
        return Task.FromResult(PubSubProducer);
    }
}
