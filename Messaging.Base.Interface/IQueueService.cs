using System;
using System.Collections.Generic;
using System.Text;

namespace Messaging.Base.Interface
{
    public interface IQueueService<TMessageQueue, TMessage>
    {
        void RegisterReceiver(IMessageReceiver<TMessageQueue, TMessage> receiver);
        void Run();
        void SendReply(Object responseObject, TMessage originalRequestMessage);
        void OnMessageReceived(TMessage receivedMessage);
    }
}
