namespace MassTransit.PubSubIntegration;

using MassTransit.Configuration;
using Transports;

public class ProcessorContextSupervisor :
    TransportPipeContextSupervisor<ProcessorContext>,
    IProcessorContextSupervisor
{
    public ProcessorContextSupervisor(
        IConnectionContextSupervisor supervisor,
        IHostConfiguration hostConfiguration,
        IReceiveSettings receiveSettings)
        : base(new ProcessorContextFactory(supervisor, hostConfiguration, receiveSettings))
    {
        supervisor.AddConsumeAgent(this);
    }
}
