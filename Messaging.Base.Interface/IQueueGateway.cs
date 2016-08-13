using System;
using System.Collections.Generic;
using System.Text;

namespace Messaging.Base
{
    public interface IQueueGateway<TMessageQueue> : IMessageCore<TMessageQueue>
    {
        void SetQueue(TMessageQueue queue);
    }
}
