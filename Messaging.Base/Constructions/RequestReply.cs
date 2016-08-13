using Messaging.Base.Construction;
using Messaging.Base.Interface;
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

    public abstract class RequestReply_Synchronous<TMessageQueue, TMessage> : RequestReply<TMessageQueue, TMessage>, IRequestReply_Sychronous
    {
        public abstract Object ProcessReceivedMessage(Object receivedMessageObject);
    }

    public abstract class RequestReply_Asynchronous<TMessageQueue, TMessage> : RequestReply<TMessageQueue, TMessage>, IRequestReply_Asychronous<TMessage>
    {
        public abstract void ProcessReceivedMessage(Object receivedMessageObject, TMessage msg);
    }
}
