namespace MassTransit.PubSubIntegration;

using Transports;

public interface PubSubSendTransportContext :
    SendTransportContext
{
    Uri HostAddress { get; }
    PubSubEndpointAddress EndpointAddress { get; }
    ISendPipe SendPipe { get; }

    Task Send(IPipe<ProducerContext> pipe, CancellationToken cancellationToken);
}
