namespace MassTransit;

using Microsoft.Extensions.DependencyInjection;
using Testing;


public static class PubSubTestHarnessExtensions
{
    public static Task<IPubSubProducer> GetProducer(this ITestHarness harness, string PubSubName)
    {
        return harness.Scope.ServiceProvider.GetRequiredService<IPubSubProducerProvider>().GetProducer(PubSubName);
    }
}
