using System;
using System.Collections.Generic;
using System.Text;

namespace Messaging.Base.Constructions
{
    public abstract class RequestReply<TMessage>
    {
        public abstract void OnMessageReceived(TMessage receivedMessage);
    }
}
