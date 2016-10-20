using System;
using System.Collections.Generic;
using System.Text;
using Messaging.Base.Constructions;

namespace Messaging.Base
{
    public interface IMessageSender<TMessageQueue, TMessage> : IMessageCore<TMessageQueue>
    {
        IReturnAddress<TMessage> AsReturnAddress();
        void Send(TMessage message);
    }
}
