namespace MassTransit.PubSubIntegration;

using Agents;
using Internals;
using MassTransit.Configuration;


public class ProcessorContextFactory :
    IPipeContextFactory<ProcessorContext>
{
    private readonly IConnectionContextSupervisor contextSupervisor;
    private readonly IHostConfiguration hostConfiguration;
    private readonly IReceiveSettings receiveSettings;

    public ProcessorContextFactory(
        IConnectionContextSupervisor contextSupervisor,
        IHostConfiguration hostConfiguration,
        IReceiveSettings receiveSettings)
    {
        this.contextSupervisor = contextSupervisor;
        this.hostConfiguration = hostConfiguration;
        this.receiveSettings = receiveSettings;
    }

    public IActivePipeContextAgent<ProcessorContext> CreateActiveContext(ISupervisor supervisor,
        PipeContextHandle<ProcessorContext> context, CancellationToken cancellationToken = new CancellationToken())
    {
        return supervisor.AddActiveContext(context, CreateSharedConnection(context.Context, cancellationToken));
    }

    IPipeContextAgent<ProcessorContext> IPipeContextFactory<ProcessorContext>.CreateContext(ISupervisor supervisor)
    {
        var asyncContext = supervisor.AddAsyncContext<ProcessorContext>();

        this.CreateProcessor(asyncContext, supervisor.Stopped);

        return asyncContext;
    }

    private static async Task<ProcessorContext> CreateSharedConnection(Task<ProcessorContext> context,
        CancellationToken cancellationToken)
    {
        return context.IsCompletedSuccessfully()
            ? new SharedProcessorContext(context.Result, cancellationToken)
            : new SharedProcessorContext(await context.OrCanceled(cancellationToken).ConfigureAwait(false), cancellationToken);
    }

    private void CreateProcessor(IAsyncPipeContextAgent<ProcessorContext> asyncContext, CancellationToken cancellationToken)
    {
        Task<ProcessorContext> Create(ConnectionContext connectionContext, CancellationToken createCancellationToken)
        {
            ProcessorContext context = new PubSubProcessorContext(
                this.hostConfiguration,
                this.receiveSettings,
                createCancellationToken);
            return Task.FromResult(context);
        }

        this.contextSupervisor.CreateAgent(asyncContext, Create, cancellationToken);
    }
}
