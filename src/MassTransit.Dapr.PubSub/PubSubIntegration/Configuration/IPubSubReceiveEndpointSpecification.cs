namespace MassTransit.PubSubIntegration.Configuration;

using Transports;

public interface IPubSubReceiveEndpointSpecification :
    IReceiveEndpointObserverConnector,
    ISpecification
{
    /// <summary>
    /// PubSub name
    /// </summary>
    string EndpointName { get; }

    public string PubSubName { get; }

    public string Topic { get; }

    public IPubSubDataReceiver Receiver { get; }

    ReceiveEndpoint CreateReceiveEndpoint(IBusInstance busInstance);
}
