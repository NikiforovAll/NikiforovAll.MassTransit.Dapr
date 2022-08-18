namespace MassTransit.PubSubIntegration.Configuration;

using Observables;
using Transports;


public class PubSubReceiveEndpointSpecification :
    IPubSubReceiveEndpointSpecification
{
    private readonly Action<IPubSubReceiveEndpointConfigurator> configure;
    private readonly ReceiveEndpointObservable endpointObservers;
    private readonly IPubSubHostConfiguration hostConfiguration;

    public PubSubReceiveEndpointSpecification(
        IPubSubHostConfiguration hostConfiguration,
        string pubSubName,
        string topic,
        Action<IPubSubReceiveEndpointConfigurator> configure)
    {
        this.hostConfiguration = hostConfiguration;
        this.PubSubName = pubSubName;
        this.Topic = topic;
        this.configure = configure;
        this.EndpointName = $"{PubSubEndpointAddress.PathPrefix}/{this.PubSubName}/{this.Topic}";

        this.endpointObservers = new ReceiveEndpointObservable();
    }

    public string EndpointName { get; }

    public string Topic { get; }

    public string PubSubName { get; }

    public IPubSubDataReceiver? Receiver { get; private set; }


    public ConnectHandle ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
    {
        return this.endpointObservers.Connect(observer);
    }

    public IEnumerable<ValidationResult> Validate()
    {
        if (string.IsNullOrWhiteSpace(this.PubSubName))
        {
            yield return this.Failure("PubSubName", "should not be empty");
        }

        if (string.IsNullOrWhiteSpace(this.Topic))
        {
            yield return this.Failure("Topic", "should not be empty");
        }
    }

    public ReceiveEndpoint CreateReceiveEndpoint(IBusInstance busInstance)
    {
        var endpointConfiguration = busInstance.HostConfiguration.CreateReceiveEndpointConfiguration(this.EndpointName);
        endpointConfiguration.ConnectReceiveEndpointObserver(this.endpointObservers);

        var configurator = new PubSubReceiveEndpointConfigurator(
            this.hostConfiguration,
            busInstance,
            endpointConfiguration,
            this.PubSubName,
            this.Topic);

        this.configure?.Invoke(configurator);

        var result = this.Validate().Concat(configurator.Validate())
            .ThrowIfContainsFailure($"{TypeCache.GetShortName(this.GetType())} configuration is invalid:");

        try
        {
            var endpoint = configurator.Build();
            this.Receiver = configurator.Receiver;

            return endpoint;
        }
        catch (Exception ex)
        {
            throw new ConfigurationException(result, "An exception occurred creating PubSub receive endpoint", ex);
        }
    }
}
