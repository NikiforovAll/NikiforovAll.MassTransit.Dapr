namespace MassTransit.PubSubIntegration.Activities;

using SagaStateMachine;


public class ProduceActivity<TSaga, TMessage> :
    IStateMachineActivity<TSaga>
    where TSaga : class, SagaStateMachineInstance
    where TMessage : class
{
    private readonly ContextMessageFactory<BehaviorContext<TSaga>, TMessage> _messageFactory;
    private readonly PubSubNameProvider<TSaga> _nameProvider;

    public ProduceActivity(PubSubNameProvider<TSaga> nameProvider, ContextMessageFactory<BehaviorContext<TSaga>, TMessage> messageFactory)
    {
        this._nameProvider = nameProvider;
        this._messageFactory = messageFactory;
    }

    public void Accept(StateMachineVisitor inspector)
    {
        inspector.Visit(this);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("produce");
    }

    public async Task Execute(BehaviorContext<TSaga> context, IBehavior<TSaga> next)
    {
        await this.Execute(context).ConfigureAwait(false);

        await next.Execute(context).ConfigureAwait(false);
    }

    public async Task Execute<T>(BehaviorContext<TSaga, T> context, IBehavior<TSaga, T> next)
        where T : class
    {
        await this.Execute(context).ConfigureAwait(false);

        await next.Execute(context).ConfigureAwait(false);
    }

    public Task Faulted<TException>(BehaviorExceptionContext<TSaga, TException> context, IBehavior<TSaga> next)
        where TException : Exception
    {
        return next.Faulted(context);
    }

    public Task Faulted<T, TException>(BehaviorExceptionContext<TSaga, T, TException> context, IBehavior<TSaga, T> next)
        where T : class
        where TException : Exception
    {
        return next.Faulted(context);
    }

    private Task Execute(BehaviorContext<TSaga> context)
    {
        return this._messageFactory.Use(context, async (ctx, s) =>
        {
            var producer = await ctx.GetProducer(ctx, this._nameProvider(ctx)).ConfigureAwait(false);

            await producer.Produce(s.Message, s.Pipe, ctx.CancellationToken).ConfigureAwait(false);
        });
    }
}


public class ProduceActivity<TSaga, TMessage, T> :
    IStateMachineActivity<TSaga, TMessage>
    where TSaga : class, SagaStateMachineInstance
    where TMessage : class
    where T : class
{
    private readonly ContextMessageFactory<BehaviorContext<TSaga, TMessage>, T> _messageFactory;
    private readonly PubSubNameProvider<TSaga, TMessage> _nameProvider;

    public ProduceActivity(PubSubNameProvider<TSaga, TMessage> nameProvider, ContextMessageFactory<BehaviorContext<TSaga, TMessage>, T> messageFactory)
    {
        this._nameProvider = nameProvider;
        this._messageFactory = messageFactory;
    }

    public void Accept(StateMachineVisitor inspector)
    {
        inspector.Visit(this);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("produce");
    }

    public async Task Execute(BehaviorContext<TSaga, TMessage> context, IBehavior<TSaga, TMessage> next)
    {
        await this._messageFactory.Use(context, async (ctx, s) =>
        {
            var producer = await ctx.GetProducer(ctx, this._nameProvider(ctx)).ConfigureAwait(false);

            await producer.Produce(s.Message, s.Pipe, ctx.CancellationToken).ConfigureAwait(false);
        }).ConfigureAwait(false);

        await next.Execute(context).ConfigureAwait(false);
    }

    public Task Faulted<TException>(BehaviorExceptionContext<TSaga, TMessage, TException> context,
        IBehavior<TSaga, TMessage> next)
        where TException : Exception
    {
        return next.Faulted(context);
    }
}
