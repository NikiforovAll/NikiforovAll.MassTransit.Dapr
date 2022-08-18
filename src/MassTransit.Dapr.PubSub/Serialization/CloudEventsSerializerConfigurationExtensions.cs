namespace MassTransit;

using Serialization;

public static class CloudEventsSerializerConfigurationExtensions
{
    /// <summary>
    /// Serialize messages using the CloudEvents message serializer
    /// </summary>
    /// <param name="configurator"></param>
    /// <param name="options">Options for the raw serializer behavior</param>
    /// <param name="isDefault">If true, set the default content type to the content type of the deserializer</param>
    public static void UseCloudEventsSerializer(
        this IBusFactoryConfigurator configurator,
        RawSerializerOptions options = RawSerializerOptions.Default,
        bool isDefault = false)
    {
        var factory = new CloudEventsMessageSerializerFactory(options);

        configurator.AddSerializer(factory);
        configurator.AddDeserializer(factory, isDefault);
    }

    /// <summary>
    /// Serialize messages using the CloudEvents message serializer
    /// </summary>
    /// <param name="configurator"></param>
    /// <param name="options">Options for the raw serializer behavior</param>
    /// <param name="isDefault">If true, set the default content type to the content type of the deserializer</param>
    public static void UseCloudEventsDeserializer(
        this IBusFactoryConfigurator configurator,
        RawSerializerOptions options = RawSerializerOptions.Default,
        bool isDefault = false)
    {
        var factory = new CloudEventsMessageSerializerFactory(options);

        configurator.AddDeserializer(factory, isDefault);
    }

    /// <summary>
    /// Serialize messages using the CloudEvents message serializer
    /// </summary>
    /// <param name="configurator"></param>
    /// <param name="options">Options for the raw serializer behavior</param>
    /// <param name="isDefault">If true, set the default content type to the content type of the deserializer</param>
    public static void UseCloudEventsSerializer(
        this IReceiveEndpointConfigurator configurator,
        RawSerializerOptions options = RawSerializerOptions.Default,
        bool isDefault = false)
    {
        var factory = new CloudEventsMessageSerializerFactory(options);

        configurator.AddSerializer(factory);
        configurator.AddDeserializer(factory, isDefault);
    }

    /// <summary>
    /// Serialize messages using the CloudEvents message serializer
    /// </summary>
    /// <param name="configurator"></param>
    /// <param name="options">Options for the raw serializer behavior</param>
    /// <param name="isDefault">If true, set the default content type to the content type of the deserializer</param>
    public static void UseCloudEventsDeserializer(
        this IReceiveEndpointConfigurator configurator,
        RawSerializerOptions options = RawSerializerOptions.Default,
        bool isDefault = false)
    {
        var factory = new CloudEventsMessageSerializerFactory(options);

        configurator.AddDeserializer(factory, isDefault);
    }
}
