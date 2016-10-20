using System;
using System.Collections.Generic;
using System.Text;
using Messaging.Base.Constructions;

namespace Messaging.Base
{
    public abstract class SenderGateway<TMessageQueue, TMessage> : IMessageSender<TMessageQueue, TMessage>
    {
        IQueueGateway<TMessageQueue> _queueGateway;

        public SenderGateway(IQueueGateway<TMessageQueue> queueGateway)
        {
            _queueGateway = queueGateway;
        }

        public TMessageQueue GetQueue()
        {
            return _queueGateway.GetQueue();
        }

        public abstract IReturnAddress<TMessage> AsReturnAddress();

        public abstract void Send(TMessage message);
    }
}
