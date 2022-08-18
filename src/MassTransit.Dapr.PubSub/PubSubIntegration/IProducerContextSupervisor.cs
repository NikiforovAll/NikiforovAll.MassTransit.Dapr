namespace MassTransit.PubSubIntegration;

using Transports;

public interface IProducerContextSupervisor :
    ITransportSupervisor<ProducerContext>
{
}
