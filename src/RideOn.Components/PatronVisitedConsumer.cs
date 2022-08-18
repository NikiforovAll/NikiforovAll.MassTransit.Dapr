namespace RideOn.Components;

using MassTransit;
using Microsoft.Extensions.Logging;
using RideOn.Contracts;

public class PatronVisitedConsumer :
    IConsumer<PatronVisited>
{
    private readonly ILogger<PatronVisitedConsumer> logger;

    public PatronVisitedConsumer(ILogger<PatronVisitedConsumer> logger)
    {
        this.logger = logger;
    }

    public Task Consume(ConsumeContext<PatronVisited> context)
    {
        this.logger.LogInformation("Patron Visited: {PatronId} {Entered} {Left} {Duration}", context.Message.PatronId,
            context.Message.Entered, context.Message.Left, context.Message.Left - context.Message.Entered);

        return Task.CompletedTask;
    }
}
