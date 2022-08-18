namespace MassTransit;

public interface PubSubSendContext :
    SendContext
{
    string PartitionId { get; set; }
    string PartitionKey { get; set; }
}


public interface PubSubSendContext<out T> :
    SendContext<T>,
    PubSubSendContext
    where T : class
{
}
