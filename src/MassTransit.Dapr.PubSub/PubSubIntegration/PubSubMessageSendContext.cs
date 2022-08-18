namespace MassTransit.PubSubIntegration;

using System.Threading;
using Context;


public class PubSubMessageSendContext<T> :
    MessageSendContext<T>,
    PubSubSendContext<T>
    where T : class
{
    public PubSubMessageSendContext(T message, CancellationToken cancellationToken)
        : base(message, cancellationToken)
    {
    }

    public string PartitionId { get; set; }
    public string PartitionKey { get; set; }
}
