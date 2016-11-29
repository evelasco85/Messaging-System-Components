using System;
using System.Collections.Generic;
using System.Text;

namespace Messaging.Base
{
    public interface IQueueService
    {
        bool Running { get; }
        void Run();
        void StopRunning();
    }

    public interface IQueueService<TMessage>
    {
        void SendReply(Object responseObject, TMessage originalRequestMessage);
    }

    public interface IQueueService<TMessageQueue, TMessage> : IQueueService<TMessage>, IQueueService
    {
        IMessageReceiver<TMessageQueue, TMessage> Receiver { get; }
        IMessageSender<TMessageQueue, TMessage> InvalidQueue { get; }
    }
}
