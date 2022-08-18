namespace MassTransit.Serialization;

using System.Net.Mime;

public class CloudEventsMessageSerializerFactory :
    ISerializerFactory
{
    private readonly Lazy<CloudEventsMessageSerializer> serializer;
    private readonly Lazy<CloudEventsMessageDeserializer> deserializer;

    public CloudEventsMessageSerializerFactory(RawSerializerOptions options = RawSerializerOptions.Default)
    {
        this.serializer = new Lazy<CloudEventsMessageSerializer>(
            () => new CloudEventsMessageSerializer(options));
        this.deserializer = new Lazy<CloudEventsMessageDeserializer>(
            () => new CloudEventsMessageDeserializer(options));
    }

    public ContentType ContentType => CloudEventsMessageSerializer.CloudEventsContentType;

    public IMessageSerializer CreateSerializer()
    {
        return this.serializer.Value;
    }

    public IMessageDeserializer CreateDeserializer()
    {
        return this.deserializer.Value;
    }
}
