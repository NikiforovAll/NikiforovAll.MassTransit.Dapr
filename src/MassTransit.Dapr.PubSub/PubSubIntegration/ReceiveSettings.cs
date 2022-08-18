namespace MassTransit.PubSubIntegration;
public interface IReceiveSettings
{
    string PubSubName { get; }
    string Topic { get; }
    int ConcurrentMessageLimit { get; }
}
