using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Base.System_Management.SmartProxy
{
    public class MessageReferenceData<TMessageQueue, TJournal> : IMessageReferenceData<TMessageQueue, TJournal>
    {
        public TJournal InternalJournal { get; set; }       //Proxy related journal
        public TJournal ExternalJournal { get; set; }       //External system journal
        public TMessageQueue OriginalReturnAddress { get; set; }
    }

    public abstract class MessageConsumer<TMessageQueue, TMessage, TJournal> : IMessageConsumer<TMessageQueue, TMessage, TJournal>
    {
        private IMessageCore<TMessageQueue> _messageQueue;

        IList<IMessageReferenceData<TMessageQueue, TJournal>> _references;

        public IList<IMessageReferenceData<TMessageQueue, TJournal>> ReferenceData
        {
            get { return _references; }
            set { _references = value; }
        }

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

            if(receiver != null)
                receiver.StartReceivingMessages();
        }

        public abstract void ProcessMessage(TMessage message);
    }
}
