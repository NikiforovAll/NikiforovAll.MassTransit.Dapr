namespace MassTransit;

using Configuration;

public static class PubSubSerializerConfigurationExtensions
{
    /// <summary>
    /// Serialize messages using the JSON serializer
    /// </summary>
    /// <param name="configurator"></param>
    public static void UseJsonSerializer(this IPubSubFactoryConfigurator configurator)
    {
        configurator.AddSerializer(new SystemTextJsonMessageSerializerFactory());
    }

    /// <summary>
    /// Serialize messages using the raw JSON message serializer
    /// </summary>
    /// <param name="configurator"></param>
    public static void UseRawJsonSerializer(this IPubSubFactoryConfigurator configurator)
    {
        configurator.AddSerializer(new SystemTextJsonRawMessageSerializerFactory());
    }
}
