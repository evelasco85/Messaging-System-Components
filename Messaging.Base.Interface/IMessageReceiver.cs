using System;
using System.Collections.Generic;
using System.Text;

namespace Messaging.Base.Interface
{
    public interface IMessageReceiver<TMessageQueue, TMessage> : IMessageCore<TMessageQueue>
    {
        MessageDelegate<TMessage> ReceiveMessageProcessor
        {
            get;
            set;
        }

        void StartReceivingMessages();
    }
}
