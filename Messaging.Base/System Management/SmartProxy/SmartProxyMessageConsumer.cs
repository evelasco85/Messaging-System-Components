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

    public abstract class SmartProxySmartProxyMessageConsumer<TMessageQueue, TMessage, TJournal> : MessageConsumer<TMessageQueue, TMessage> , ISmartProxyMessageConsumer<TMessageQueue, TMessage, TJournal>
    {
        IList<IMessageReferenceData<TMessageQueue, TJournal>> _references;

        public IList<IMessageReferenceData<TMessageQueue, TJournal>> ReferenceData
        {
            get { return _references; }
            set { _references = value; }
        }

        public SmartProxySmartProxyMessageConsumer(IMessageSender<TMessageQueue, TMessage> sender) : base(sender)
        {
        }

        public SmartProxySmartProxyMessageConsumer(IMessageReceiver<TMessageQueue, TMessage> monitorReceiver) : base(monitorReceiver)
        {
        }
    }
}
