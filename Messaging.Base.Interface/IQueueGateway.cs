namespace Messaging.Base
{
    public interface IQueueGateway<TMessageQueue> : IMessageCore<TMessageQueue>
    {
        void SetQueue(TMessageQueue queue);
    }
}
