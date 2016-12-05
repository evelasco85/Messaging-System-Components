using Messaging.Base.Constructions;

namespace Messaging.Base
{
    public interface IMessageReceiver
    {
        string QueueName { get; }
        bool Started { get; }
        void StartReceivingMessages();
        void StopReceivingMessages();
        void SetupReceiver();
    }

    public interface IMessageReceiver<TMessage> : IMessageReceiver
    {
        MessageDelegate<TMessage> ReceiveMessageProcessor
        {
            get;
            set;
        }


        IReturnAddress<TMessage> AsReturnAddress();
    }

    public interface IMessageReceiver<TMessageQueue, TMessage> : IMessageReceiver<TMessage>, IMessageCore<TMessageQueue>
    {
    }
}
