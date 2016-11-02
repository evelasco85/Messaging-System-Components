using System;
using System.Collections.Generic;
using System.Text;

namespace Messaging.Base
{
    public interface IQueueService<TMessageQueue, TMessage>
    {
        IMessageReceiver<TMessageQueue, TMessage> Receiver { get; }
        IMessageSender<TMessageQueue, TMessage> InvalidQueue { get; }
        void Run();
        void StopRunning();
        void SendReply(Object responseObject, TMessage originalRequestMessage);
    }
}
