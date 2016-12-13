using System;
using System.Collections.Generic;
using Messaging.Base.Constructions;

namespace Messaging.Base
{

    /// <summary>
    /// Abstract implementation for sending messages
    /// </summary>
    /// <typeparam name="TMessageQueue">Type of message queue</typeparam>
    /// <typeparam name="TMessage">Type of message to send</typeparam>
    public abstract class SenderGateway<TMessageQueue, TMessage> : IMessageSender<TMessageQueue, TMessage>
    {
        IQueueGateway<TMessageQueue> _queueGateway;

        public string QueueName
        {
            get { return _queueGateway.QueueName; }
        }

        public SenderGateway(IQueueGateway<TMessageQueue> queueGateway)
        {
            _queueGateway = queueGateway;

            SetupSender();
        }

        public TMessageQueue GetQueue()
        {
            return _queueGateway.GetQueue();
        }

        public abstract IReturnAddress<TMessage> AsReturnAddress();

        public abstract TMessage Send(TMessage message);
        public abstract TMessage Send<TEntity>(TEntity message);
        public abstract TMessage Send<TEntity>(TEntity entity, Action<AssignSenderPropertyDelegate> AssignProperty);
        public abstract TMessage Send<TEntity>(TEntity entity, IReturnAddress<TMessage> returnAddress);
        public abstract TMessage Send<TEntity>(TEntity entity, IReturnAddress<TMessage> returnAddress, Action<AssignSenderPropertyDelegate> AssignProperty);
        public abstract void SetupSender();
    }
}
