namespace MassTransit;

using System;
using System.Runtime.Serialization;


[Serializable]
public class PubSubConnectionException :
    ConnectionException
{
    public PubSubConnectionException()
    {
    }

    public PubSubConnectionException(string message)
        : base(message)
    {
    }

    public PubSubConnectionException(string message, Exception innerException)
        : base(message, innerException, IsExceptionTransient(innerException))
    {
    }

    protected PubSubConnectionException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    private static bool IsExceptionTransient(Exception exception)
    {
        return exception switch
        {
            UnauthorizedAccessException _ => false,
            _ => true
        };
    }
}
