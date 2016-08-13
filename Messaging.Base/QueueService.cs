using Messaging.Base.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Messaging.Base
{
    public abstract class QueueService<TMessageQueue, TMessage> : IQueueService<TMessageQueue, TMessage>
    {
        public abstract IMessageReceiver<TMessageQueue, TMessage> Receiver { get; }
        public abstract void Run();
        public abstract void SendReply(Object responseObject, TMessage originalRequestMessage);
    }
}
