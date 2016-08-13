using System;
using System.Collections.Generic;
using System.Text;

namespace Messaging.Base
{
    //Used by both sender and receiver messaging queues
    public abstract class QueueGateway<TMessageQueue> : IQueueGateway<TMessageQueue>
    {
        public abstract TMessageQueue GetQueue();
        public abstract void SetQueue(TMessageQueue queue);
    }
}
