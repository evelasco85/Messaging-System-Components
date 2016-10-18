using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Base.System_Management.SmartProxy
{
    public class SmartProxyBase<TMessageQueue, TMessage, TJournal> : ISmartProxyBase
    {
        ISmartProxyRequestConsumer<TMessageQueue, TMessage, TJournal> _requestConsumer;
        ISmartProxyReplyConsumer<TMessageQueue, TMessage, TJournal> _replyConsumer;
        
        protected IList<IMessageReferenceData<TMessageQueue, TJournal>> _referenceData;

        public SmartProxyBase(
            ISmartProxyRequestConsumer<TMessageQueue, TMessage, TJournal> requestConsumer,
            ISmartProxyReplyConsumer<TMessageQueue, TMessage, TJournal> replyConsumer)
        {
            _referenceData = new List<IMessageReferenceData<TMessageQueue, TJournal>>();

            _requestConsumer = requestConsumer;
            _replyConsumer = replyConsumer;

            _requestConsumer.ReferenceData = _referenceData;
            _replyConsumer.ReferenceData = _referenceData;
        }

        //Start listening incoming messages
        public virtual void Process()
        {
            _requestConsumer.Process();     
            _replyConsumer.Process();
        }
    }
}
