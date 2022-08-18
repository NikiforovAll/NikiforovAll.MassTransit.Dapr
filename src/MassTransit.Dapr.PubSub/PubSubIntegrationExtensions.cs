namespace MassTransit;

using DependencyInjection;
using PubSubIntegration.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class PubSubIntegrationExtensions
{
    public static void UsingDaprPubSub(
        this IRiderRegistrationConfigurator configurator,
        Action<IRiderRegistrationContext, IPubSubFactoryConfigurator> configure)
    {
        if (configurator == null)
        {
            throw new ArgumentNullException(nameof(configurator));
        }

        var factory = new PubSubRegistrationRiderFactory(configure);
        configurator.SetRiderFactory(factory);

        configurator.TryAddScoped<IPubSubRider, IPubSubProducerProvider>(GetCurrentProducerProvider);
    }

    public static void UsingDaprPubSub<TBus>(
        this IRiderRegistrationConfigurator<TBus> configurator,
        Action<IRiderRegistrationContext, IPubSubFactoryConfigurator> configure)
        where TBus : class, IBus
    {
        if (configurator == null)
        {
            throw new ArgumentNullException(nameof(configurator));
        }

        var factory = new PubSubRegistrationRiderFactory(configure);
        configurator.SetRiderFactory(factory);

        configurator.TryAddScoped<IPubSubRider, Bind<TBus, IPubSubProducerProvider>>((rider, provider) =>
            Bind<TBus>.Create(GetCurrentProducerProvider(rider, provider)));
    }

    private static IPubSubProducerProvider GetCurrentProducerProvider(
        IPubSubRider rider,
        IServiceProvider provider)
    {
        var contextProvider = provider.GetService<ScopedConsumeContextProvider>();
        if (contextProvider != null)
        {
            return contextProvider.HasContext
                ? rider.GetProducerProvider(contextProvider.GetContext())
                : rider.GetProducerProvider();
        }

        return rider.GetProducerProvider(provider.GetService<ConsumeContext>());
    }
}
