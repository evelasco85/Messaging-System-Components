using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Base.System_Management.SmartProxy
{
    public interface IMessageConsumer<TMessageQueue, TMessage, TJournal>
    {
        IList<MessageReferenceData<TMessageQueue, TMessage, TJournal>> ReferenceData { get; set; }

        void Process();
        void ProcessMessage(TMessage message);
    }

    public interface IRequestMessageConsumer<TMessageQueue, TMessage, TJournal> : IMessageConsumer<TMessageQueue, TMessage, TJournal>
    {
        TJournal ConstructJournalReference(TMessage message);
    }

    public interface IReplyMessageConsumer<TMessageQueue, TMessage, TJournal> : IMessageConsumer<TMessageQueue, TMessage, TJournal>
    {
        Func<TJournal, bool> GetJournalLookupCondition(TMessage message);
    }

    public class MessageReferenceData<TMessageQueue, TMessage, TJournal>
    {
        public TJournal InternalJournal { get; set; }       //Correlation and Id
        public TJournal ExternalJournal { get; set; }       //Correlation and Id
        public TMessageQueue OriginalReturnAddress { get; set; }
    }

    public abstract class MessageConsumer<TMessageQueue, TMessage, TJournal> : IMessageConsumer<TMessageQueue, TMessage, TJournal>
    {
        private IMessageCore<TMessageQueue> _messageQueue;

        IList<MessageReferenceData<TMessageQueue, TMessage, TJournal>> _references;

        public IList<MessageReferenceData<TMessageQueue, TMessage, TJournal>> ReferenceData
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
