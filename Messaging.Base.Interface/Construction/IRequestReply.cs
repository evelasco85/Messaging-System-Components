using System;
using System.Collections.Generic;
using System.Text;

namespace Messaging.Base.Construction
{
    public interface IRequestReply<TMessageQueue, TMessage>
    {
        bool Running { get; }
        IQueueService<TMessageQueue, TMessage> QueueService { get; set; }
        void OnMessageReceived(TMessage receivedMessage);
        void SendReply(Object responseObject, TMessage originalRequestMessage);
        void Run();
        void StopRunning();
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
