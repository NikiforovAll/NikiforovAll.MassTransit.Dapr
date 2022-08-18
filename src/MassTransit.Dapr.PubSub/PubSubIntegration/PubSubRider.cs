namespace MassTransit.PubSubIntegration;

using Configuration;
using MassTransit.Middleware;
using Transports;

public class PubSubRider :
    IPubSubRider
{
    private readonly IBusInstance busInstance;
    private readonly IReceiveEndpointCollection endpoints;
    private readonly IPubSubHostConfiguration hostConfiguration;
    private readonly IPubSubProducerProvider producerProvider;
    private readonly IRiderRegistrationContext registrationContext;

    public IList<IPubSubReceiveEndpointSpecification> EndpointSpecs { get; }

    public PubSubRider(
        IPubSubHostConfiguration hostConfiguration,
        IBusInstance busInstance,
        IReceiveEndpointCollection endpoints,
        IList<IPubSubReceiveEndpointSpecification> endpointSpecs,
        IPubSubProducerProvider producerProvider,
        IRiderRegistrationContext registrationContext)
    {
        this.hostConfiguration = hostConfiguration;
        this.busInstance = busInstance;
        this.endpoints = endpoints;
        this.EndpointSpecs = endpointSpecs;
        this.producerProvider = producerProvider;
        this.registrationContext = registrationContext;
    }

    public IPubSubProducerProvider GetProducerProvider(ConsumeContext consumeContext = default)
    {
        return consumeContext == null
            ? this.producerProvider
            : new ConsumeContextPubSubProducerProvider(this.producerProvider, consumeContext);
    }

    public HostReceiveEndpointHandle ConnectPubSubEndpoint(string PubSubName, string consumerGroup,
        Action<IRiderRegistrationContext, IPubSubReceiveEndpointConfigurator> configure)
    {
        var specification = this.hostConfiguration.CreateSpecification(PubSubName, consumerGroup, configurator =>
        {
            configure?.Invoke(this.registrationContext, configurator);
        });

        this.endpoints.Add(specification.EndpointName, specification.CreateReceiveEndpoint(this.busInstance));

        return this.endpoints.Start(specification.EndpointName);
    }

    public RiderHandle Start(CancellationToken cancellationToken = default)
    {
        var endpointsHandle = this.endpoints.StartEndpoints(cancellationToken);

        var ready = endpointsHandle.Length == 0 ? Task.CompletedTask : this.hostConfiguration.ConnectionContextSupervisor.Ready;

        var agent = new RiderAgent(this.hostConfiguration.ConnectionContextSupervisor, this.endpoints, ready);

        return new Handle(endpointsHandle, agent);
    }

    private class RiderAgent :
        Agent
    {
        private readonly IReceiveEndpointCollection _endpoints;
        private readonly IConnectionContextSupervisor _supervisor;

        public RiderAgent(IConnectionContextSupervisor supervisor, IReceiveEndpointCollection endpoints, Task ready)
        {
            this._supervisor = supervisor;
            this._endpoints = endpoints;

            this.SetReady(ready);
            this.SetCompleted(this._supervisor.Completed);
        }

        protected override async Task StopAgent(StopContext context)
        {
            await this._endpoints.Stop(context.CancellationToken).ConfigureAwait(false);
            await this._supervisor.Stop(context).ConfigureAwait(false);
            await base.StopAgent(context).ConfigureAwait(false);
        }
    }

    private class Handle :
        RiderHandle
    {
        private readonly IAgent _agent;
        private readonly HostReceiveEndpointHandle[] _endpoints;

        public Handle(HostReceiveEndpointHandle[] endpoints, IAgent agent)
        {
            this._endpoints = endpoints;
            this._agent = agent;
        }

        public Task Ready => this.ReadyOrNot(this._endpoints.Select(x => x.Ready));

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return this._agent.Stop("EvenHub stopped", cancellationToken);
        }

        private async Task ReadyOrNot(IEnumerable<Task<ReceiveEndpointReady>> endpoints)
        {
            var readyTasks = endpoints as Task<ReceiveEndpointReady>[] ?? endpoints.ToArray();
            foreach (var ready in readyTasks)
            {
                await ready.ConfigureAwait(false);
            }

            await this._agent.Ready.ConfigureAwait(false);

            await Task.WhenAll(readyTasks).ConfigureAwait(false);
        }
    }
}
