namespace MassTransit.PubSubIntegration;

using Transports;

public interface IProcessorContextSupervisor :
    ITransportSupervisor<ProcessorContext>
{
}
