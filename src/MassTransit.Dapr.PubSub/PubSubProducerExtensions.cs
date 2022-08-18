namespace MassTransit;

public static class PubSubProducerExtensions
{
    public static Task<IPubSubProducer> GetProducer(this IPubSubProducerProvider producerProvider, string pubSubName)
    {
        if (producerProvider == null)
        {
            throw new ArgumentNullException(nameof(producerProvider));
        }

        if (string.IsNullOrWhiteSpace(pubSubName))
        {
            throw new ArgumentNullException(nameof(pubSubName));
        }

        return producerProvider.GetProducer(new Uri($"topic:{pubSubName}"));
    }
}
