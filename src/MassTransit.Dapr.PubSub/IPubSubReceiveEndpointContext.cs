namespace MassTransit;

using PubSubIntegration;
using Transports;

public interface IPubSubReceiveEndpointContext :
    ReceiveEndpointContext
{
    IProcessorContextSupervisor ContextSupervisor { get; }
}
