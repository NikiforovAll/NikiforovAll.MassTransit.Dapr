namespace MassTransit;

public interface PubSubConsumeContext
{
    IReadOnlyDictionary<string, object> SystemProperties { get; }
    IDictionary<string, object> Properties { get; }
}
