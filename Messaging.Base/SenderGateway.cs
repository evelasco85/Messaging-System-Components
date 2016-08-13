using System;
using System.Collections.Generic;
using System.Text;

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

        public abstract void Send(TMessage message);
    }
}
