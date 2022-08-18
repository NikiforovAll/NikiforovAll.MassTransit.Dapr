using Dapr.Client;
using MassTransit;
using RideOn.Components;
using RideOn.Components.Consumers;
using RideOn.Contracts;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddDaprClient();

services.AddMassTransit(x =>
{
    x.SetSnakeCaseEndpointNameFormatter();
    x.AddConsumer<MyEventConsumer>();

    x.UsingRabbitMq();
    x.AddRider(rider =>
    {
        rider.SetKebabCaseEndpointNameFormatter();

        rider.AddSagaStateMachine<PatronStateMachine, PatronState, PatronStateDefinition>()
            .InMemoryRepository();

        rider.AddConsumersFromNamespaceContaining<MyEventConsumer>();

        rider.UsingDaprPubSub((context, pubsub) =>
        {
            pubsub.Subscribe("pubsub", "consumer", cfg =>
            {
                cfg.ConfigureConsumer<MyEventConsumer>(context);
            });

            pubsub.Subscribe("pubsub", "saga", cfg =>
            {
                cfg.ConfigureSaga<PatronState>(context);
            });
        });
    });
});

services.RemoveMassTransitHostedService();

var app = builder.Build();

app.MapPost("create-dapr", async (DaprClient daprClient) =>
{
    var @event = Sample();
    await daprClient.PublishEventAsync(@event.PubSub, @event.Topic, @event);
});

app.MapPost("create-mt", async (IPublishEndpoint publishEndpoint) =>
    await publishEndpoint.Publish(Sample()));

app.MapPost("create-sage", async (bool enter, bool visit, DaprClient daprClient) =>
{
    var patronId = Guid.NewGuid();

    if (enter)
    {
        await daprClient.PublishEventAsync("pubsub", "saga", new PatronEntered()
        {
            PatronId = patronId,
            Timestamp = DateTime.Now,
        });
    }

    if (visit)
    {
        await daprClient.PublishEventAsync("pubsub", "saga", new PatronLeft()
        {
            PatronId = patronId,
            Timestamp = DateTime.Now,
        });
    }
});

// TODO: blocks app startup if rabbitmq is unavailable
await app.UsePubSubEndpoints();

app.Run();

static MyEvent Sample() => new(Guid.NewGuid().ToString(), "pubsub", "consumer");
