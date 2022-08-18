namespace MassTransit.PubSubIntegration;

using Transports;


public class ProducerContextSupervisor :
    TransportPipeContextSupervisor<ProducerContext>,
    IProducerContextSupervisor
{
    public ProducerContextSupervisor(IConnectionContextSupervisor contextSupervisor, string pubSubName, ISerialization serializers)
        : base(new ProducerContextFactory(contextSupervisor, pubSubName, serializers))
    {
        contextSupervisor.AddSendAgent(this);
    }
}
