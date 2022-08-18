namespace RideOn.Components;

using MassTransit;
using MassTransit.Middleware;
using RideOn.Contracts;

public class PatronStateDefinition :
    SagaDefinition<PatronState>
{
    private readonly IPartitioner partition;

    public PatronStateDefinition()
    {
        this.partition = new Partitioner(64, new Murmur3UnsafeHashGenerator());
    }

    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<PatronState> sagaConfigurator)
    {
        sagaConfigurator.Message<PatronEntered>(x => x.UsePartitioner(this.partition, m => m.Message.PatronId));
        sagaConfigurator.Message<PatronLeft>(x => x.UsePartitioner(this.partition, m => m.Message.PatronId));

        endpointConfigurator.UseMessageRetry(r => r.Intervals(20, 50, 100, 1000, 5000));
    }
}
