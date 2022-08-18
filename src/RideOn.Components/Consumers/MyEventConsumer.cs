namespace RideOn.Components.Consumers;

using MassTransit;

public record class MyEvent(string Id, string PubSub, string Topic);

public class MyEventConsumerDefinition :
    ConsumerDefinition<MyEventConsumer>
{
    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<MyEventConsumer> consumerConfigurator)
    {
        endpointConfigurator.UseMessageRetry(r => r.Intervals(500, 1000));
    }
}

public class MyEventConsumer :
    IConsumer<MyEvent>
{
    public Task Consume(ConsumeContext<MyEvent> context)
    {
        return Task.CompletedTask;
    }
}
