using System;
using System.Collections.Generic;
using System.Text;
using Messaging.Base.Constructions;

namespace Messaging.Base
{
    public interface IMessageReceiver<TMessageQueue, TMessage> : IMessageCore<TMessageQueue>
    {
        MessageDelegate<TMessage> ReceiveMessageProcessor
        {
            get;
            set;
        }

        IReturnAddress<TMessage> AsReturnAddress();
        void StartReceivingMessages();
        void StopReceivingMessages();
        void SetupReceiver();
    }
}
