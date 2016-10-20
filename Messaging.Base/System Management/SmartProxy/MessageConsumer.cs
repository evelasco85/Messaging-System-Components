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

        public MessageConsumer(IMessageSender<TMessageQueue, TMessage> sender)
        {
            _messageQueue = sender;
        }

        public MessageConsumer(IMessageReceiver<TMessageQueue, TMessage> receiver)
        {
            receiver.ReceiveMessageProcessor += new MessageDelegate<TMessage>(ProcessMessage);

            _messageQueue = receiver;
        }

        public void Process()
        {
            IMessageReceiver<TMessageQueue, TMessage> receiver = ((IMessageReceiver<TMessageQueue, TMessage>)_messageQueue);

            if (receiver != null)
                receiver.StartReceivingMessages();
        }

        public abstract void ProcessMessage(TMessage message);
    }
}
