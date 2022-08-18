namespace MassTransit.PubSubIntegration;

using Configuration;
using MassTransit.Configuration;
using Transports;
using Util;


public class PubSubReceiveEndpointContext :
    BaseReceiveEndpointContext,
    IPubSubReceiveEndpointContext
{
    private readonly IBusInstance busInstance;
    private readonly Recycle<IProcessorContextSupervisor> contextSupervisor;

    public PubSubReceiveEndpointContext(IPubSubHostConfiguration hostConfiguration, IBusInstance busInstance,
        IReceiveEndpointConfiguration endpointConfiguration, IReceiveSettings receiveSettings)
        : base(busInstance.HostConfiguration, endpointConfiguration)
    {
        this.busInstance = busInstance;
        this.contextSupervisor = new Recycle<IProcessorContextSupervisor>(() =>
            new ProcessorContextSupervisor(
                hostConfiguration.ConnectionContextSupervisor, busInstance.HostConfiguration, receiveSettings));
    }

    public override void AddSendAgent(IAgent agent)
    {
        this.contextSupervisor.Supervisor.AddSendAgent(agent);
    }

    public override void AddConsumeAgent(IAgent agent)
    {
        this.contextSupervisor.Supervisor.AddConsumeAgent(agent);
    }

    public override Exception ConvertException(Exception exception, string message)
    {
        return new PubSubConnectionException(message, exception);
    }

    public IProcessorContextSupervisor ContextSupervisor => this.contextSupervisor.Supervisor;

    protected override ISendTransportProvider CreateSendTransportProvider()
    {
        throw new NotSupportedException();
    }

    protected override IPublishTransportProvider CreatePublishTransportProvider()
    {
        throw new NotSupportedException();
    }

    protected override IPublishEndpointProvider CreatePublishEndpointProvider()
    {
        return this.busInstance.Bus;
    }

    protected override ISendEndpointProvider CreateSendEndpointProvider()
    {
        return this.busInstance.Bus;
    }
}
