using Messaging.Base.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Messaging.Base.Constructions
{
    public abstract class RequestReply<TMessageQueue, TMessage>
    {
        IQueueService<TMessageQueue, TMessage> _queueService;

        public IQueueService<TMessageQueue, TMessage> QueueService
        {
            get { return _queueService; }
            set { _queueService = value; }
        }

        public void RegisterReceiveMessageProcessor()
        {
            _queueService.Receiver.ReceiveMessageProcessor += new MessageDelegate<TMessage>(OnMessageReceived);
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
}
