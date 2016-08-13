using System;
using System.Collections.Generic;
using System.Text;

namespace Messaging.Base.Interface
{
    public interface IQueueService<TMessageQueue, TMessage>
    {
        IMessageReceiver<TMessageQueue, TMessage> Receiver { get; }
        void Run();
        void SendReply(Object responseObject, TMessage originalRequestMessage);
    }
}
