namespace MassTransit.Serialization;

using System.Buffers;
using System.Net.Mime;
using System.Text.Json;
using CloudNative.CloudEvents;
using CloudNative.CloudEvents.SystemTextJson;
using MassTransit.Context;

public class CloudEventsMessageDeserializer : IMessageDeserializer
{
    public static readonly ContentType CloudEventsContentType = new("application/json");

    private readonly Dictionary<string, Type> types = new();
    private readonly RawSerializerOptions options;

    public CloudEventsMessageDeserializer(
        RawSerializerOptions options = RawSerializerOptions.Default)
    {
        this.options = options;
    }

    void IProbeSite.Probe(ProbeContext context)
    {
    }

    ConsumeContext IMessageDeserializer.Deserialize(ReceiveContext receiveContext)
    {
        var formatter = new JsonEventFormatter();
        var message = formatter.DecodeStructuredModeMessage(
            (ReadOnlyMemory<byte>)receiveContext.GetBody(), null, null);

        return new CloudEventContext(receiveContext, message, this.types, this.Options);
    }

    public ContentType ContentType { get; set; } = CloudEventsContentType;


    public JsonSerializerOptions Options { get; } = new() { PropertyNameCaseInsensitive = true };



    public void AddType<T>(string type) =>
        this.types[type] = typeof(T);

    public SerializerContext Deserialize(
        MessageBody body, Headers headers, Uri? destinationAddress = null)
    {
        throw new NotImplementedException();
    }

    public MessageBody GetMessageBody(string text) => new StringMessageBody(text);

    private sealed class CloudEventContext : DeserializerConsumeContext
    {
        private readonly CloudEvent cloudEvent;
        private readonly Dictionary<string, Type> mappings;
        private readonly JsonSerializerOptions options;

        public CloudEventContext(
            ReceiveContext receiveContext,
            CloudEvent cloudEvent,
            Dictionary<string, Type> mappings,
            JsonSerializerOptions options)
                : base(receiveContext)
        {
            this.cloudEvent = cloudEvent;
            this.mappings = mappings;
            this.options = options;
        }

        public override bool HasMessageType(Type messageType) =>
            true;

        public override bool TryGetMessage<T>(out ConsumeContext<T>? consumeContext)
        {
            try
            {
                var message = ToObject<T>(this.cloudEvent, this.Type<T>(), this.options);

                consumeContext = new MessageConsumeContext<T>(this, message);
                return true;
            }
            catch (NotSupportedException)
            {
                consumeContext = null;
                return false;
            }

            static T ToObject<T>(CloudEvent element, Type type, JsonSerializerOptions? options = null)
            {
                // It is currently not possible to deserialize from a JsonElement: https://github.com/dotnet/runtime/issues/31274#issuecomment-804360901
                var buffer = new ArrayBufferWriter<byte>();
                using (var writer = new Utf8JsonWriter(buffer))
                {
                    ((JsonElement)element!.Data!).WriteTo(writer);
                }

                return (T)JsonSerializer.Deserialize(buffer.WrittenSpan, type, options)!;
            }
        }

        public override Guid? MessageId => Guid.TryParse(this.cloudEvent.Id, out var result) ? result : null;
        public override Guid? RequestId { get; }
        public override Guid? CorrelationId { get; }
        public override Guid? ConversationId { get; }
        public override Guid? InitiatorId { get; }
        public override DateTime? ExpirationTime { get; }
        public override Uri? SourceAddress => this.cloudEvent.Source;
        public override Uri? DestinationAddress { get; }
        public override Uri? ResponseAddress { get; }
        public override Uri? FaultAddress { get; }
        public override DateTime? SentTime => this.cloudEvent.Time?.DateTime;
        public override Headers Headers { get; } = new DictionarySendHeaders(new Dictionary<string, object>());

        public override HostInfo? Host { get; }
        public override IEnumerable<string> SupportedMessageTypes { get; } = Enumerable.Empty<string>();

        private Type Type<T>() =>
            this.mappings.TryGetValue(this.cloudEvent.Type!, out var result) ? result : typeof(T);
    }
}
