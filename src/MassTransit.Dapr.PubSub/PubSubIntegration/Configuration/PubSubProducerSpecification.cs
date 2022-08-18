namespace MassTransit.PubSubIntegration.Configuration;

using System;
using System.Collections.Generic;
using MassTransit.Configuration;
using Observables;
using Transports;


public class PubSubProducerSpecification :
    IPubSubProducerConfigurator,
    IPubSubProducerSpecification
{
    private readonly IPubSubHostConfiguration _hostConfiguration;
    private readonly SendObservable _sendObservers;
    private readonly ISerializationConfiguration _serializationConfiguration;
    private Action<ISendPipeConfigurator> _configureSend;

    public PubSubProducerSpecification(IPubSubHostConfiguration hostConfiguration)
    {
        this._hostConfiguration = hostConfiguration;
        this._serializationConfiguration = new SerializationConfiguration();
        this._sendObservers = new SendObservable();
    }

    public ConnectHandle ConnectSendObserver(ISendObserver observer)
    {
        return this._sendObservers.Connect(observer);
    }

    public void ConfigureSend(Action<ISendPipeConfigurator> callback)
    {
        this._configureSend = callback ?? throw new ArgumentNullException(nameof(callback));
    }

    public void AddSerializer(ISerializerFactory factory, bool isSerializer = true)
    {
        this._serializationConfiguration.AddSerializer(factory, isSerializer);
    }

    public IEnumerable<ValidationResult> Validate()
    {
        yield return this.Success("Empty");
    }

    public IPubSubProducerProvider CreateProducerProvider(IBusInstance busInstance)
    {
        var sendConfiguration = new SendPipeConfiguration(busInstance.HostConfiguration.Topology.SendTopology);
        this._configureSend?.Invoke(sendConfiguration.Configurator);
        var sendPipe = sendConfiguration.CreatePipe();

        return new PubSubProducerProvider(this._hostConfiguration, busInstance, sendPipe, this._sendObservers,
            this._serializationConfiguration.CreateSerializerCollection());
    }
}
