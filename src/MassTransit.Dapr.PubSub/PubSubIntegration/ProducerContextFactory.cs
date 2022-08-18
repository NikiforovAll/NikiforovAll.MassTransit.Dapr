namespace MassTransit.PubSubIntegration;

using Agents;
using Internals;


public class ProducerContextFactory :
    IPipeContextFactory<ProducerContext>
{
    private readonly IConnectionContextSupervisor contextSupervisor;
    private readonly string pubSubName;
    private readonly ISerialization serializers;

    public ProducerContextFactory(IConnectionContextSupervisor contextSupervisor, string pubSubName, ISerialization serializers)
    {
        this.contextSupervisor = contextSupervisor;
        this.pubSubName = pubSubName;
        this.serializers = serializers;
    }

    public IActivePipeContextAgent<ProducerContext> CreateActiveContext(ISupervisor supervisor,
        PipeContextHandle<ProducerContext> context, CancellationToken cancellationToken = new CancellationToken())
    {
        return supervisor.AddActiveContext(context, CreateSharedConnection(context.Context, cancellationToken));
    }

    IPipeContextAgent<ProducerContext> IPipeContextFactory<ProducerContext>.CreateContext(ISupervisor supervisor)
    {
        var asyncContext = supervisor.AddAsyncContext<ProducerContext>();

        this.CreateProcessor(asyncContext, supervisor.Stopped);

        return asyncContext;
    }

    private static async Task<ProducerContext> CreateSharedConnection(Task<ProducerContext> context,
        CancellationToken cancellationToken)
    {
        return context.IsCompletedSuccessfully()
            ? new SharedProducerContext(context.Result, cancellationToken)
            : new SharedProducerContext(await context.OrCanceled(cancellationToken).ConfigureAwait(false), cancellationToken);
    }

    private void CreateProcessor(IAsyncPipeContextAgent<ProducerContext> asyncContext, CancellationToken cancellationToken)
    {
        Task<ProducerContext> Create(ConnectionContext connectionContext, CancellationToken createCancellationToken)
        {
            ProducerContext context = new PubSubProducerContext(this.serializers, cancellationToken);
            return Task.FromResult(context);
        }

        this.contextSupervisor.CreateAgent(asyncContext, Create, cancellationToken);
    }
}
