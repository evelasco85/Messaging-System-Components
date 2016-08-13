using System;
using System.Collections.Generic;
using System.Text;

namespace Messaging.Base.Construction
{
    public interface IRequestReply<TMessageQueue, TMessage>
    {
        IQueueService<TMessageQueue, TMessage> QueueService { get; set; }
        void OnMessageReceived(TMessage receivedMessage);
        void SendReply(Object responseObject, TMessage originalRequestMessage);
        void Run();
    }

    public interface IRequestReply_Synchronous
    {
        Object ProcessRequestMessage(Object receivedMessageObject);
    }

    public interface IRequestReply_Asynchronous<TMessage>
    {
        void ProcessRequestMessage(Object receivedMessageObject, TMessage msg);
    }
}
