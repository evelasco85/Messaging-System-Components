using System;
using System.Collections.Generic;
using System.Text;
using Messaging.Base.Constructions;

namespace Messaging.Base
{
    public abstract class ReceiverGateway<TMessageQueue, TMessage> : IMessageReceiver<TMessageQueue, TMessage>
    {
        IQueueGateway<TMessageQueue> _queueGateway;

        public abstract MessageDelegate<TMessage> ReceiveMessageProcessor
        {
            get;
            set;
        }

        public ReceiverGateway(IQueueGateway<TMessageQueue> queueGateway)
        {
            _queueGateway = queueGateway;
        }

        public TMessageQueue GetQueue()
        {
            return _queueGateway.GetQueue();
        }

        public abstract IReturnAddress<TMessage> AsReturnAddress();

        public abstract void StartReceivingMessages();
    }
}
