namespace MassTransit.PubSubIntegration;

public class PubSubEventArgs
{
    public string? Body { get; set; }

    public IDictionary<string, object>? Headers { get; set; }
    public string? ContentType { get; set; }
}
