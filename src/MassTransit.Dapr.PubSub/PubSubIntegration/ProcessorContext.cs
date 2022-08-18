namespace MassTransit.PubSubIntegration;

public interface ProcessorContext :
    PipeContext
{
    IReceiveSettings ReceiveSettings { get; }
}
