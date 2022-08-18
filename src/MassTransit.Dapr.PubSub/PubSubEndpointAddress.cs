namespace MassTransit;

using System.Diagnostics;


[DebuggerDisplay("{" + nameof(DebuggerDisplay) + "}")]
public readonly struct PubSubEndpointAddress
{
    public const string PathPrefix = "dapr";

    public readonly string PubSubName;

    public readonly string Host;
    public readonly string Scheme;
    public readonly int? Port;

    public PubSubEndpointAddress(Uri hostAddress, Uri address)
    {
        this.Host = default;
        this.PubSubName = default;
        this.Scheme = default;
        this.Port = default;

        var scheme = address.Scheme.ToLowerInvariant();
        switch (scheme)
        {
            case "topic":
                ParseLeft(hostAddress, out this.Scheme, out this.Host, out this.Port);
                this.PubSubName = address.AbsolutePath;
                break;
            default:
            {
                if (string.Equals(address.Scheme, hostAddress.Scheme, StringComparison.InvariantCultureIgnoreCase))
                {
                    ParseLeft(hostAddress, out this.Scheme, out this.Host, out this.Port);
                    this.PubSubName = address.AbsolutePath.Replace($"{PathPrefix}/", "");
                }
                else
                {
                    throw new ArgumentException($"The address scheme is not supported: {address.Scheme}", nameof(address));
                }

                break;
            }
        }
    }

    public PubSubEndpointAddress(Uri hostAddress, string pubSubName)
    {
        ParseLeft(hostAddress, out this.Scheme, out this.Host, out this.Port);

        this.PubSubName = pubSubName;
    }

    private static void ParseLeft(Uri address, out string scheme, out string host, out int? port)
    {
        scheme = address.Scheme;
        host = address.Host;
        port = address.Port;
    }

    public static implicit operator Uri(in PubSubEndpointAddress address)
    {
        var builder = new UriBuilder
        {
            Scheme = address.Scheme,
            Host = address.Host,
            Port = address.Port ?? 0,
            Path = $"{PathPrefix}/{address.PubSubName}"
        };

        return builder.Uri;
    }

    private Uri DebuggerDisplay => this;
}
