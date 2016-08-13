using System;
using System.Collections.Generic;
using System.Text;

namespace Messaging.Base
{
    public interface IMessageSender<TMessageQueue, TMessage> : IMessageCore<TMessageQueue>
    {
        void Send(TMessage message);
    }
}
