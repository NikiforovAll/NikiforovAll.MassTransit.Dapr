namespace MassTransit.PubSubIntegration;

using Transports;

public interface IConnectionContextSupervisor :
    ITransportSupervisor<ConnectionContext>
{
}
