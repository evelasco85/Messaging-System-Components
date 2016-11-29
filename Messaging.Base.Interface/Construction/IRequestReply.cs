using System;
using System.Collections.Generic;
using System.Text;

namespace Messaging.Base.Construction
{
    public interface IRequestReply
    {
        bool Running { get; }
        void Run();
        void StopRunning();
    }

    public interface IRequestReply<TMessage> : IRequestReply
    {
        void OnMessageReceived(TMessage receivedMessage);
        void SendReply(Object responseObject, TMessage originalRequestMessage);
    }

    public interface IRequestReply<TMessageQueue, TMessage> : IRequestReply<TMessage>
    {
        IQueueService<TMessageQueue, TMessage> QueueService { get; set; }
    }

    public interface IRequestReply_Synchronous : IRequestReply
    {
        Object ProcessRequestMessage(Object receivedMessageObject);
    }

    public interface IRequestReply_Asynchronous<TMessage> : IRequestReply<TMessage>
    {
        void ProcessRequestMessage(Object receivedMessageObject, TMessage msg);
    }
}
