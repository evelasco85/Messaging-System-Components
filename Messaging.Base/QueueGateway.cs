namespace Messaging.Base
{
    /// <summary>
    /// Abstract implementation for setting(storing) and retrieving message queue
    /// </summary>
    /// <typeparam name="TMessageQueue">Type of message queue to store</typeparam>
    public abstract class QueueGateway<TMessageQueue> : IQueueGateway<TMessageQueue>
    {
        public abstract string QueueName { get; }
        public abstract TMessageQueue GetQueue();
        public abstract void SetQueue(TMessageQueue queue);
    }
}
