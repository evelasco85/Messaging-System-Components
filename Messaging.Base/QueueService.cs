using Messaging.Base.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Messaging.Base
{
    public abstract class QueueService<TMessageQueue, TMessage> : IQueueService<TMessageQueue, TMessage>
    {
        public abstract void RegisterReceiver(IMessageReceiver<TMessageQueue, TMessage> receiver);
        public abstract void Run();
        public abstract void SendReply(Object responseObject, TMessage originalRequestMessage);
        public abstract void OnMessageReceived(TMessage receivedMessage);
    }
}
