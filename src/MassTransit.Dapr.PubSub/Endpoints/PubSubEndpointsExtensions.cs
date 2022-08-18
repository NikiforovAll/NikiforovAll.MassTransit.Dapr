namespace MassTransit;

using MassTransit.Dapr.PubSub;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

public static class PubSubEndpointsExtensions
{
    public static async Task<IEndpointRouteBuilder> UsePubSubEndpoints(this IEndpointRouteBuilder builder)
    {
        var depot = builder.ServiceProvider.GetRequiredService<IBusDepot>();
        await depot.Start(default);

        var descriptors = builder.ServiceProvider.GetRequiredService<IPubSubRider>().EndpointSpecs;

        builder.DataSources.Add(new PubSubDataSource(descriptors));
        return builder;
    }
}
