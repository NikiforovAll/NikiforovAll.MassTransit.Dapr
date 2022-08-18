namespace RideOn.Components;

using MassTransit;
using RideOn.Contracts;

public sealed class PatronStateMachine :
    MassTransitStateMachine<PatronState>
{
    public PatronStateMachine()
    {
        this.Event(() => this.Entered, x => x.CorrelateById(m => m.Message.PatronId));
        this.Event(() => this.Left, x => x.CorrelateById(m => m.Message.PatronId));

        this.InstanceState(x => x.CurrentState, this.Tracking);

        this.Initially(
            this.When(this.Entered)
                .Then(context => context.Saga.Entered = context.Message.Timestamp)
                .TransitionTo(this.Tracking),
            this.When(this.Left)
                .Then(context => context.Saga.Left = context.Message.Timestamp)
                .TransitionTo(this.Tracking)
        );

        this.During(this.Tracking,
            this.When(this.Entered)
                .Then(context => context.Saga.Entered = context.Message.Timestamp),
            this.When(this.Left)
                .Then(context => context.Saga.Left = context.Message.Timestamp)
        );

        this.CompositeEvent(() => this.Visited, x => x.VisitedStatus, CompositeEventOptions.IncludeInitial, this.Entered, this.Left);

        this.DuringAny(
            this.When(this.Visited)
                .Then(context => Console.WriteLine("Visited: {0}", context.Saga.CorrelationId))
                .PublishAsync(context => context.Init<PatronVisited>(new
                {
                    PatronId = context.Saga.CorrelationId,
                    context.Saga.Entered,
                    context.Saga.Left
                }))
                // .Produce(context => context.Init<PatronVisited>(new
                // {
                //     PatronId = context.Saga.CorrelationId,
                //     context.Saga.Entered,
                //     context.Saga.Left
                // }))
                .Finalize()
        );

        this.SetCompletedWhenFinalized();
    }

    public State Tracking { get; private set; }
    public Event<PatronEntered> Entered { get; private set; }
    public Event<PatronLeft> Left { get; private set; }
    public Event Visited { get; private set; }
}
