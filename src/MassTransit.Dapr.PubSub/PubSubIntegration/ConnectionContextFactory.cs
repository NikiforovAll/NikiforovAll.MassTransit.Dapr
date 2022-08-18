namespace MassTransit.PubSubIntegration;

using Agents;
using Internals;

public class ConnectionContextFactory :
    IPipeContextFactory<ConnectionContext>
{
    public ConnectionContextFactory()
    {
    }

    IPipeContextAgent<ConnectionContext> IPipeContextFactory<ConnectionContext>.CreateContext(ISupervisor supervisor)
    {
        var context = Task.FromResult(this.CreateConnectionContext(supervisor));

        var contextHandle = supervisor.AddContext(context);

        return contextHandle;
    }

    IActivePipeContextAgent<ConnectionContext> IPipeContextFactory<ConnectionContext>.CreateActiveContext(ISupervisor supervisor,
        PipeContextHandle<ConnectionContext> context, CancellationToken cancellationToken)
    {
        return supervisor.AddActiveContext(context, CreateSharedConnectionContext(context.Context, cancellationToken));
    }

    private static async Task<ConnectionContext> CreateSharedConnectionContext(Task<ConnectionContext> context, CancellationToken cancellationToken)
    {
        return context.IsCompletedSuccessfully()
            ? new SharedConnectionContext(context.Result, cancellationToken)
            : new SharedConnectionContext(await context.OrCanceled(cancellationToken).ConfigureAwait(false), cancellationToken);
    }

    private ConnectionContext CreateConnectionContext(ISupervisor supervisor)
    {
        return new PubSubConnectionContext(supervisor.Stopped);
    }
}
