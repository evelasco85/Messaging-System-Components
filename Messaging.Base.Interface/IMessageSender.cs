using System;
using System.Collections.Generic;
using System.Text;

namespace Messaging.Base.Interface
{
    public interface IMessageSender<TMessageQueue, TMessage> : IMessageCore<TMessageQueue>
    {
        void Send(TMessage message);
    }
}
