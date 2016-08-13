using System;
using System.Collections.Generic;
using System.Text;

namespace Messaging.Base
{
    public abstract class QueueService<TMessageQueue, TMessage> : IQueueService<TMessageQueue, TMessage>
    {
        IMessageReceiver<TMessageQueue, TMessage> _receiver;

        public QueueService(IMessageReceiver<TMessageQueue, TMessage> receiver)
        {
            _receiver = receiver;
        }

        public IMessageReceiver<TMessageQueue, TMessage> Receiver
        {
            get { return _receiver; }
            set { _receiver = value; }
        }

        public void Run()
        {
            _receiver.StartReceivingMessages();
        }

        public abstract void SendReply(Object responseObject, TMessage originalRequestMessage);
    }
}
