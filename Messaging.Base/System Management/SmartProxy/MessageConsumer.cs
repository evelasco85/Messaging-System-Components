using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Base.System_Management.SmartProxy
{
    public abstract class MessageConsumer<TMessageQueue, TMessage> : IMessageConsumer<TMessageQueue, TMessage>
    {
        private IMessageCore<TMessageQueue> _messageQueue;
        bool _processStarted = false;

        public bool ProcessStarted
        {
            get { return _processStarted; }
        }

        public MessageConsumer(IMessageCore<TMessageQueue> messageQueue)
        {
            _messageQueue = messageQueue;
        }

        public MessageConsumer(IMessageSender<TMessageQueue, TMessage> sender) : this((IMessageCore<TMessageQueue>)sender)
        {
        }

        public MessageConsumer(IMessageReceiver<TMessageQueue, TMessage> receiver)
            : this((IMessageCore<TMessageQueue>)receiver)
        {
            receiver.ReceiveMessageProcessor += new MessageDelegate<TMessage>(ProcessMessage);
        }

        public bool Process()
        {
            IMessageReceiver<TMessageQueue, TMessage> receiver = ((IMessageReceiver<TMessageQueue, TMessage>)_messageQueue);

            if ((receiver != null) && (!_processStarted))
            {
                receiver.StartReceivingMessages();

                _processStarted = true;
            }

            return _processStarted;
        }

        public abstract void ProcessMessage(TMessage message);

        public virtual void StopProcessing()
        {
            IMessageReceiver<TMessageQueue, TMessage> receiver = ((IMessageReceiver<TMessageQueue, TMessage>)_messageQueue);

            if ((receiver != null) && (_processStarted))
            {
                receiver.StopReceivingMessages();

                _processStarted = false;
            }
        }
    }
}
