using System.Collections.Generic;

namespace Messaging.Base.System_Management.SmartProxy
{
    public class MessageReferenceData<TMessageQueue, TJournal> : IMessageReferenceData<TMessageQueue, TJournal>
    {
        public TJournal InternalJournal { get; set; }       //Proxy related journal
        public TJournal ExternalJournal { get; set; }       //External system journal
        public TMessageQueue OriginalReturnAddress { get; set; }
    }

    public abstract class SmartProxyMessageConsumer<TMessageQueue, TMessage, TJournal> : MessageConsumer<TMessage> , ISmartProxyMessageConsumer<TMessageQueue, TMessage, TJournal>
    {
        IList<IMessageReferenceData<TMessageQueue, TJournal>> _references;

        public IList<IMessageReferenceData<TMessageQueue, TJournal>> ReferenceData
        {
            get { return _references; }
            set { _references = value; }
        }

        public SmartProxyMessageConsumer(IMessageReceiver<TMessage> receiver) : base(receiver)
        {
        }
    }
}
