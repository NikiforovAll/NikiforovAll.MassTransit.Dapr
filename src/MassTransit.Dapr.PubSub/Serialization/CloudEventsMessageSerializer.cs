namespace MassTransit.Serialization;

using System.Net.Mime;
using System.Text.Json;
using CloudNative.CloudEvents;
using CloudNative.CloudEvents.SystemTextJson;

public class CloudEventsMessageSerializer : RawMessageSerializer, IMessageSerializer
{
    public static readonly ContentType CloudEventsContentType = new("application/json");

    private readonly Dictionary<Type, string> types = new();
    private readonly RawSerializerOptions options;

    public CloudEventsMessageSerializer(
        RawSerializerOptions options = RawSerializerOptions.Default)
    {
        this.options = options;
    }

    public void Serialize<T>(Stream stream, SendContext<T> context) where T : class
    {
        stream.Write(this.CloudEventToMemory(context).Span);
    }

    public ContentType ContentType { get; set; } = CloudEventsContentType;

    public JsonSerializerOptions Options { get; } = new();

    public void AddType<T>(string type) =>
        this.types[typeof(T)] = type;

    private string Type(Type type) =>
        this.types.TryGetValue(type, out var result) ? result : type.Name;

    public MessageBody GetMessageBody<T>(SendContext<T> context) where T : class
    {

        if (this.options.HasFlag(RawSerializerOptions.AddTransportHeaders))
        {
            this.SetRawMessageHeaders<T>(context);
        }

        return new MemoryMessageBody(this.CloudEventToMemory(context));
    }

    private ReadOnlyMemory<byte> CloudEventToMemory<T>(SendContext<T> context) where T : class
    {
        var cloudEvent = new CloudEvent(CloudEventsSpecVersion.Default)
        {
            Data = context.Message,
            Source = context.SourceAddress ?? new Uri("cloudeventify:masstransit"),
            Id = context.MessageId.ToString(),
            Type = this.Type(context.Message.GetType()),
            Time = context.SentTime
        };
        var formatter = new JsonEventFormatter(this.Options, new JsonDocumentOptions());
        return formatter.EncodeStructuredModeMessage(cloudEvent, out _);
    }
}
