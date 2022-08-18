namespace MassTransit.PubSubIntegration.Configuration;

using MassTransit.Configuration;
using Middleware;
using Transports;


public class PubSubReceiveEndpointConfigurator :
    ReceiverConfiguration,
    IPubSubReceiveEndpointConfigurator,
    IReceiveSettings
{
    private readonly IBusInstance busInstance;
    private readonly IReceiveEndpointConfiguration endpointConfiguration;
    private readonly IPubSubHostConfiguration hostConfiguration;
    private readonly PipeConfigurator<ProcessorContext> processorConfigurator;

    public PubSubReceiveEndpointConfigurator(
        IPubSubHostConfiguration hostConfiguration,
        IBusInstance busInstance,
        IReceiveEndpointConfiguration endpointConfiguration,
        string topic, string consumerGroup)
        : base(endpointConfiguration)
    {
        this.hostConfiguration = hostConfiguration;
        this.busInstance = busInstance;
        this.endpointConfiguration = endpointConfiguration;

        this.PubSubName = topic;
        this.Topic = consumerGroup;

        this.ConcurrentMessageLimit = 1;

        this.UseCloudEventsSerializer(isDefault: true);

        this.processorConfigurator = new PipeConfigurator<ProcessorContext>();
    }

    int IReceiveSettings.ConcurrentMessageLimit => this.Transport.GetConcurrentMessageLimit();

    public IPubSubDataReceiver? Receiver { get; private set; }

    public string Topic { get; }
    public string PubSubName { get; }

    public ReceiveEndpoint Build()
    {
        IPubSubReceiveEndpointContext CreateContext()
        {
            var builder = new PubSubReceiveEndpointBuilder(
                this.hostConfiguration, this.busInstance, this.endpointConfiguration, this);

            foreach (var specification in this.Specifications)
            {
                specification.Configure(builder);
            }

            return builder.CreateReceiveEndpointContext();
        }

        var context = CreateContext();

        var receiver = new PubSubDataReceiver(context, this.ConcurrentMessageLimit ?? 1);
        this.Receiver = receiver;

        this.processorConfigurator.UseFilter(new PubSubConsumerFilter(context, receiver));

        var processorPipe = this.processorConfigurator.Build();

        var transport = new ReceiveTransport<ProcessorContext>(
            this.busInstance.HostConfiguration,
            context,
            () => context.ContextSupervisor,
            processorPipe);

        return new ReceiveEndpoint(transport, context);
    }
}
