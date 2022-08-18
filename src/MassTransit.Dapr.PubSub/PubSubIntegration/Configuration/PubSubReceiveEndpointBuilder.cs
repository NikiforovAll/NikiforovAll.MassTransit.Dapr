namespace MassTransit.PubSubIntegration.Configuration;

using MassTransit.Configuration;
using Transports;


public class PubSubReceiveEndpointBuilder :
    ReceiveEndpointBuilder
{
    private readonly IBusInstance _busInstance;
    private readonly IReceiveEndpointConfiguration _configuration;
    private readonly IPubSubHostConfiguration _hostConfiguration;
    private readonly IReceiveSettings _receiveSettings;

    public PubSubReceiveEndpointBuilder(
        IPubSubHostConfiguration hostConfiguration,
        IBusInstance busInstance,
        IReceiveEndpointConfiguration configuration,
        IReceiveSettings receiveSettings)
        : base(configuration)
    {
        this._hostConfiguration = hostConfiguration;
        this._busInstance = busInstance;
        this._configuration = configuration;
        this._receiveSettings = receiveSettings;
    }

    public IPubSubReceiveEndpointContext CreateReceiveEndpointContext()
    {
        var context = new PubSubReceiveEndpointContext(
            this._hostConfiguration,
            this._busInstance,
            this._configuration,
            this._receiveSettings);

        context.GetOrAddPayload(() => this._busInstance.HostConfiguration.Topology);
        context.AddOrUpdatePayload(() => this._receiveSettings, _ => this._receiveSettings);

        return context;
    }
}
