namespace MassTransit.Dapr.PubSub;

using System.Linq;
using System.Text;
using MassTransit.PubSubIntegration;
using MassTransit.PubSubIntegration.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.Primitives;

public class PubSubDataSource : EndpointDataSource
{
    private readonly object @lock = new();

    private IReadOnlyList<Endpoint> endpoints;

    private CancellationTokenSource cancellationTokenSource;

    private IChangeToken changeToken;

    public PubSubDataSource(IEnumerable<IPubSubReceiveEndpointSpecification> specs) =>
        this.SetEndpoints(MakeEndpoints(specs));

    private static IReadOnlyList<Endpoint> MakeEndpoints(IEnumerable<IPubSubReceiveEndpointSpecification> specs)
    {
        var endpoints = specs.Select(spec => CreateEndpoint(
                spec.EndpointName,
                context => HandleMessage(spec, context)))
            .Concat(new Endpoint[]{ CreateEndpoint(
                "dapr/subscribe",
                async context => await context.Response.WriteAsJsonAsync(specs.Select(s =>
                new {
                    pubsubname = s.PubSubName,
                    topic = s.Topic,
                    route = s.EndpointName
                })))})
            .ToList();

        return endpoints;
    }

    private static async Task HandleMessage(IPubSubReceiveEndpointSpecification spec, HttpContext context)
    {
        // context.Request.EnableBuffering();

        // Leave the body open so the next middleware can read it.
        using var reader = new StreamReader(
            context.Request.Body,
            encoding: Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false);

        var body = await reader.ReadToEndAsync();
        var eventArgs = new PubSubEventArgs
        {
            Body = body,
            Headers = context.Request.Headers.ToDictionary(x => x.Key, y => y.Value as object),
            ContentType = context.Request.ContentType,
        };
        await spec.Receiver.EnqueueMessage(eventArgs);
    }

    private static Endpoint CreateEndpoint(string pattern, RequestDelegate requestDelegate) =>
        new RouteEndpointBuilder(
            requestDelegate: requestDelegate,
            routePattern: RoutePatternFactory.Parse(pattern),
            order: 0)
        .Build();

    public override IChangeToken GetChangeToken() => this.changeToken;

    public override IReadOnlyList<Endpoint> Endpoints => this.endpoints;

    public void SetEndpoints(IReadOnlyList<Endpoint> endpoints)
    {
        lock (this.@lock)
        {
            var oldCancellationTokenSource = this.cancellationTokenSource;

            this.endpoints = endpoints;

            this.cancellationTokenSource = new CancellationTokenSource();
            this.changeToken = new CancellationChangeToken(this.cancellationTokenSource.Token);

            oldCancellationTokenSource?.Cancel();
        }
    }
}
