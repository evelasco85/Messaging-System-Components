using Messaging.Base.Constructions;

namespace Messaging.Base
{
    public interface IMessageSender
    {
        string QueueName { get; }
        void SetupSender();
        void Send<TEntity>(TEntity message);
    }

    public interface IMessageSender<TMessage> : IMessageSender
    {
        IReturnAddress<TMessage> AsReturnAddress();
        void Send(TMessage message);
    }

    public interface IMessageSender<TMessageQueue, TMessage> : IMessageCore<TMessageQueue>, IMessageSender<TMessage>
    {
    }
}
