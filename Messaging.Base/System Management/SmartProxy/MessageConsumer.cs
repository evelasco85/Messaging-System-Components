using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Base.System_Management.SmartProxy
{
    public class MessageReferenceData<TMessageQueue, TMessage>
    {
        public string ID { get; set; }
        public string CorrID { get; set; }
        public IMessageSender<TMessageQueue, TMessage> ReplyQueue { get; set; }
    }

    public interface IMessageConsumer<TMessageQueue, TMessage>
    {
        IList<MessageReferenceData<TMessageQueue, TMessage>> ReferenceData { get; set; }

        void Process();
        void ProcessMessage(TMessage message);
        MessageReferenceData<TMessageQueue, TMessage> GetReferenceData(TMessage message);

    }

    public abstract class MessageConsumer<TMessageQueue, TMessage> : IMessageConsumer<TMessageQueue, TMessage>
    {
        private IMessageCore<TMessageQueue> _messageQueue;

        IList<MessageReferenceData<TMessageQueue, TMessage>> _referenceData;

        public IList<MessageReferenceData<TMessageQueue, TMessage>> ReferenceData
        {
            get { return _referenceData; }
            set { _referenceData = value; }
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
            if(_messageQueue.GetType() == typeof(IMessageReceiver<TMessageQueue, TMessage>))
                ((IMessageReceiver<TMessageQueue, TMessage>)_messageQueue).StartReceivingMessages();
        }

        public abstract void ProcessMessage(TMessage message);
        public abstract MessageReferenceData<TMessageQueue, TMessage> GetReferenceData(TMessage message);
    }
}
