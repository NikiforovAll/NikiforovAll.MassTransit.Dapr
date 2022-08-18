namespace MassTransit.PubSubIntegration;

using Transports;

public class ConnectionContextSupervisor :
    TransportPipeContextSupervisor<ConnectionContext>,
    IConnectionContextSupervisor
{
    public ConnectionContextSupervisor()
        : base(new ConnectionContextFactory())
    {
    }
}
