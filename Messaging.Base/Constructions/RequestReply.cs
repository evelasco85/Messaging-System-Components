using Messaging.Base.Construction;
using System;
using System.Collections.Generic;
using System.Text;

namespace Messaging.Base.Constructions
{
    public abstract class RequestReply<TMessageQueue, TMessage> : IRequestReply<TMessageQueue, TMessage>
    {
        IQueueService<TMessageQueue, TMessage> _queueService;

        public IQueueService<TMessageQueue, TMessage> QueueService
        {
            get { return _queueService; }
            set
            {
                _queueService = value;

                RegisterReceiveMessageProcessor(_queueService);
            }
        }

        void RegisterReceiveMessageProcessor(IQueueService<TMessageQueue, TMessage> queueService)
        {
            if ((queueService != null) && (queueService.Receiver != null))
                queueService.Receiver.ReceiveMessageProcessor += new MessageDelegate<TMessage>(OnMessageReceived);
        }

        public abstract void OnMessageReceived(TMessage receivedMessage);

        public void SendReply(Object responseObject, TMessage originalRequestMessage)
        {
            _queueService.SendReply(responseObject, originalRequestMessage);
        }

        public void Run()
        {
            _queueService.Run();
        }
    }

    public abstract class RequestReply_Synchronous<TMessageQueue, TMessage> : RequestReply<TMessageQueue, TMessage>, IRequestReply_Synchronous
    {
        public abstract Object ProcessRequestMessage(Object receivedMessageObject);
    }

    public abstract class RequestReply_Asynchronous<TMessageQueue, TMessage> : RequestReply<TMessageQueue, TMessage>, IRequestReply_Asynchronous<TMessage>
    {
        public abstract void ProcessRequestMessage(Object receivedMessageObject, TMessage msg);
    }
}
