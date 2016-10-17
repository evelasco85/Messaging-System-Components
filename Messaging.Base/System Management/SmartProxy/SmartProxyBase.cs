using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Base.System_Management.SmartProxy
{
    public interface ISmartProxyBase
    {
        void Process();
    }

    public class SmartProxyBase<TMessageQueue, TMessage, TJournal> : ISmartProxyBase
    {
        ISmartProxyRequestConsumer<TMessageQueue, TMessage, TJournal> _requestConsumer;
        ISmartProxyReplyConsumer<TMessageQueue, TMessage, TJournal> _replyConsumer;
        
        protected IList<MessageReferenceData<TMessageQueue, TMessage, TJournal>> _referenceData;

        public SmartProxyBase(
            ISmartProxyRequestConsumer<TMessageQueue, TMessage, TJournal> requestConsumer,
            ISmartProxyReplyConsumer<TMessageQueue, TMessage, TJournal> replyConsumer)
        {
            _referenceData = new SynchronizedCollection<MessageReferenceData<TMessageQueue, TMessage, TJournal>>();

            _requestConsumer = requestConsumer;
            _replyConsumer = replyConsumer;

            _requestConsumer.ReferenceData = _referenceData;
            _replyConsumer.ReferenceData = _referenceData;
        }

        public virtual void Process()
        {
            _requestConsumer.Process();
            _replyConsumer.Process();
        }
    }
}
