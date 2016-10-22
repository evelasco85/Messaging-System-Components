using System;
using System.Collections.Generic;
using System.Text;

namespace Messaging.Base
{
    /// <summary>
    /// Abstract implementation for receiving and forwarding of message
    /// </summary>
    /// <typeparam name="TMessageQueue">Type of message queue to use</typeparam>
    /// <typeparam name="TMessage">Type of message to use</typeparam>
    public abstract class QueueService<TMessageQueue, TMessage> : IQueueService<TMessageQueue, TMessage>
    {
        IMessageReceiver<TMessageQueue, TMessage> _receiver;
        private IMessageSender<TMessageQueue, TMessage> _invalidQueue;

        public IMessageSender<TMessageQueue, TMessage> InvalidQueue
        {
            get { return _invalidQueue; }
        }

        public QueueService(IMessageReceiver<TMessageQueue, TMessage> receiver, IMessageSender<TMessageQueue, TMessage> invalidQueue)
        {
            _receiver = receiver;
            _invalidQueue = invalidQueue;
        }

        public IMessageReceiver<TMessageQueue, TMessage> Receiver
        {
            get { return _receiver; }
            set { _receiver = value; }
        }

        public void Run()
        {
            _receiver.StartReceivingMessages();
        }

        public abstract void SendReply(Object responseObject, TMessage originalRequestMessage);
    }
}
