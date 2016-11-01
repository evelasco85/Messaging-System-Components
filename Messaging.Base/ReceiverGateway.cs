using System;
using System.Collections.Generic;
using System.Text;
using Messaging.Base.Constructions;

namespace Messaging.Base
{
    /// <summary>
    /// Abstract implementation for receiving messages
    /// </summary>
    /// <typeparam name="TMessageQueue">Type of message queue to listen from</typeparam>
    /// <typeparam name="TMessage">Type of message to receive</typeparam>
    public abstract class ReceiverGateway<TMessageQueue, TMessage> : IMessageReceiver<TMessageQueue, TMessage>
    {
        IQueueGateway<TMessageQueue> _queueGateway;

        public abstract MessageDelegate<TMessage> ReceiveMessageProcessor
        {
            get;
            set;
        }

        public ReceiverGateway(IQueueGateway<TMessageQueue> queueGateway)
        {
            _queueGateway = queueGateway;

            SetupReceiver();
        }

        public TMessageQueue GetQueue()
        {
            return _queueGateway.GetQueue();
        }

        public abstract IReturnAddress<TMessage> AsReturnAddress();

        public abstract void StartReceivingMessages();
        public abstract void SetupReceiver();
    }
}
