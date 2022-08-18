namespace MassTransit.PubSubIntegration.Configuration;

using MassTransit.Configuration;
using Transports;

public class PubSubBusInstanceSpecification :
    IBusInstanceSpecification
{
    private readonly IRiderRegistrationContext context;
    private readonly IPubSubHostConfiguration hostConfiguration;

    public PubSubBusInstanceSpecification(IRiderRegistrationContext context, IPubSubHostConfiguration hostConfiguration)
    {
        this.context = context;
        this.hostConfiguration = hostConfiguration;
    }

    public IEnumerable<ValidationResult> Validate()
    {
        return this.hostConfiguration.Validate();
    }

    public void Configure(IBusInstance busInstance)
    {
        var rider = this.hostConfiguration.Build(this.context, busInstance);
        busInstance.Connect<IPubSubRider>(rider);
    }
}
