namespace MassTransit.PubSubIntegration.Configuration;

using MassTransit.Configuration;
using Observables;
using Transports;
using Util;

public class PubSubFactoryConfigurator :
    IPubSubFactoryConfigurator,
    IPubSubHostConfiguration
{
    private readonly Recycle<IConnectionContextSupervisor> connectionContextSupervisor;
    private readonly ReceiveEndpointObservable endpointObservers;
    private readonly List<IPubSubReceiveEndpointSpecification> endpoints;
    private readonly PubSubProducerSpecification producerSpecification;

    public PubSubFactoryConfigurator()
    {
        this.endpointObservers = new ReceiveEndpointObservable();
        this.endpoints = new List<IPubSubReceiveEndpointSpecification>();

        this.producerSpecification = new PubSubProducerSpecification(this);

        this.connectionContextSupervisor = new Recycle<IConnectionContextSupervisor>(() =>
            new ConnectionContextSupervisor());
    }

    public ConnectHandle ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
    {
        return this.endpointObservers.Connect(observer);
    }

    public void ReceiveEndpoint(string pubSubName, string topic, Action<IPubSubReceiveEndpointConfigurator> configure)
    {
        var specification = this.CreateSpecification(pubSubName, topic, configure);
        this.endpoints.Add(specification);
    }

    public void AddSerializer(ISerializerFactory factory, bool isSerializer = true)
    {
        this.producerSpecification.AddSerializer(factory, isSerializer);
    }

    public ConnectHandle ConnectSendObserver(ISendObserver observer)
    {
        return this.producerSpecification.ConnectSendObserver(observer);
    }

    public void ConfigureSend(Action<ISendPipeConfigurator> callback)
    {
        this.producerSpecification.ConfigureSend(callback);
    }

    public IPubSubReceiveEndpointSpecification CreateSpecification(
        string pubSubName,
        string topic,
        Action<IPubSubReceiveEndpointConfigurator> configure)
    {
        if (string.IsNullOrWhiteSpace(pubSubName))
        {
            throw new ArgumentException(nameof(pubSubName));
        }

        if (string.IsNullOrWhiteSpace(topic))
        {
            throw new ArgumentException(nameof(topic));
        }

        var specification = new PubSubReceiveEndpointSpecification(this, pubSubName, topic, configure);
        specification.ConnectReceiveEndpointObserver(this.endpointObservers);
        return specification;
    }

    public IConnectionContextSupervisor ConnectionContextSupervisor => this.connectionContextSupervisor.Supervisor;

    public IPubSubRider Build(IRiderRegistrationContext context, IBusInstance busInstance)
    {
        this.ConnectSendObserver(busInstance.HostConfiguration.SendObservers);

        var endpoints = new ReceiveEndpointCollection();
        foreach (var endpoint in this.endpoints)
        {
            endpoints.Add(endpoint.EndpointName, endpoint.CreateReceiveEndpoint(busInstance));
        }

        var producerProvider = this.producerSpecification.CreateProducerProvider(busInstance);

        var rider = new PubSubRider(
            this,
            busInstance,
            endpoints,
            this.endpoints,
            new CachedPubSubProducerProvider(producerProvider),
            context);

        return rider;
    }

    public IEnumerable<ValidationResult> Validate()
    {
        foreach (var kv in this.endpoints.GroupBy(x => x.EndpointName)
            .ToDictionary(x => x.Key, x => x.ToArray()))
        {
            if (kv.Value.Length > 1)
            {
                yield return this.Failure($"PubSub: {kv.Key} was added more than once.");
            }

            foreach (var result in kv.Value.SelectMany(x => x.Validate()))
            {
                yield return result;
            }
        }

        foreach (var result in this.producerSpecification.Validate())
        {
            yield return result;
        }
    }

    public IBusInstanceSpecification Build(IRiderRegistrationContext context)
    {
        return new PubSubBusInstanceSpecification(context, this);
    }
}
