namespace MassTransit.PubSubIntegration.Activities;

using SagaStateMachine;

internal static class ProducerFactoryExtensions
{
    internal static Task<IPubSubProducer> GetProducer<T>(this BehaviorContext<T> context, ConsumeContext consumeContext, string PubSubName)
        where T : class, ISaga
    {
        var factory = context.GetStateMachineActivityFactory();

        var rider = factory.GetService<IPubSubRider>(context) ?? throw new ProduceException("PubSubRider not found");

        var producerProvider = rider.GetProducerProvider(consumeContext);

        return producerProvider.GetProducer(PubSubName);
    }
}
