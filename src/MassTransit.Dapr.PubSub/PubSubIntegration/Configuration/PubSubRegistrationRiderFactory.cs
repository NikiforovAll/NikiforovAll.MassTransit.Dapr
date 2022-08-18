namespace MassTransit.PubSubIntegration.Configuration;

using DependencyInjection;
using MassTransit.Configuration;


public class PubSubRegistrationRiderFactory :
    IRegistrationRiderFactory<IPubSubRider>
{
    private readonly Action<IRiderRegistrationContext, IPubSubFactoryConfigurator> configure;

    public PubSubRegistrationRiderFactory(
        Action<IRiderRegistrationContext,
        IPubSubFactoryConfigurator> configure)
    {
        this.configure = configure;
    }

    public IBusInstanceSpecification CreateRider(IRiderRegistrationContext context)
    {
        var configurator = new PubSubFactoryConfigurator();

        this.configure?.Invoke(context, configurator);

        return configurator.Build(context);
    }
}
