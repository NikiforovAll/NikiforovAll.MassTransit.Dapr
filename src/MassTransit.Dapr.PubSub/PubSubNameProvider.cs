namespace MassTransit;

/// <summary>
/// Return the name of the event hub
/// </summary>
/// <typeparam name="TSaga"></typeparam>
/// <param name="context"></param>
/// <returns></returns>
public delegate string PubSubNameProvider<TSaga>(BehaviorContext<TSaga> context)
    where TSaga : class, ISaga;


/// <summary>
/// Return the name of the event hub
/// </summary>
/// <typeparam name="TSaga"></typeparam>
/// <typeparam name="TMessage"></typeparam>
/// <param name="context"></param>
/// <returns></returns>
public delegate string PubSubNameProvider<TSaga, in TMessage>(BehaviorContext<TSaga, TMessage> context)
    where TSaga : class, ISaga
    where TMessage : class;
