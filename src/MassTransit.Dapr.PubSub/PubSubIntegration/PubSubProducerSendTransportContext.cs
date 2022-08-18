namespace MassTransit.PubSubIntegration;

using Configuration;
using MassTransit.Configuration;
using Transports;
using Util;

internal class PubSubProducerSendTransportContext :
    BaseSendTransportContext,
    PubSubSendTransportContext
{
    private readonly IHostConfiguration configuration;
    private readonly Recycle<IProducerContextSupervisor> producerContextSupervisor;

    public PubSubProducerSendTransportContext(IPubSubHostConfiguration hostConfiguration, ISendPipe sendPipe,
        IHostConfiguration configuration, Uri endpointAddress, ISerialization serialization)
        : base(configuration, serialization)
    {
        this.configuration = configuration;
        this.SendPipe = sendPipe;

        this.EndpointAddress = new PubSubEndpointAddress(this.HostAddress, endpointAddress);

        this.producerContextSupervisor = new Recycle<IProducerContextSupervisor>(() =>
            new ProducerContextSupervisor(hostConfiguration.ConnectionContextSupervisor, this.EndpointAddress.PubSubName, serialization));
    }

    public Uri HostAddress => this.configuration.HostAddress;

    public PubSubEndpointAddress EndpointAddress { get; }

    public ISendPipe SendPipe { get; }

    public Task Send(IPipe<ProducerContext> pipe, CancellationToken cancellationToken)
    {
        var supervisor = this.producerContextSupervisor.Supervisor;

        return this.configuration.Retry(() => supervisor.Send(pipe, cancellationToken), supervisor, cancellationToken);
    }

    public override string EntityName => this.EndpointAddress.PubSubName;
    public override string ActivitySystem => "dapr-hub";

    public override Task<SendContext<T>> CreateSendContext<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
    {
        throw new NotImplementedByDesignException("Event Hub is a producer, not an outbox compatible transport");
    }
}
