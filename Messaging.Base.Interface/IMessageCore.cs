namespace Messaging.Base
{
    public interface IMessageCore<TMessageQueue>
    {
        string QueueName { get; }
        TMessageQueue GetQueue();
    }
}
