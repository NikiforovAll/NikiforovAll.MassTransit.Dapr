namespace MassTransit.PubSubIntegration;

using Transports;

public interface IPubSubDataReceiver :
    IAgent,
    DeliveryMetrics
{
    Task Start();

    Task EnqueueMessage(PubSubEventArgs eventArgs);
}
