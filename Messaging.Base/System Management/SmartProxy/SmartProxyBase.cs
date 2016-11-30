using System.Collections.Generic;

namespace Messaging.Base.System_Management.SmartProxy
{
    public class SmartProxyBase<TMessageQueue, TMessage, TJournal> : ISmartProxyBase
    {
        ISmartProxyRequestSmartProxyConsumer<TMessageQueue, TMessage, TJournal> _requestSmartProxyConsumer;
        ISmartProxyReplySmartProxyConsumer<TMessageQueue, TMessage, TJournal> _replySmartProxyConsumer;
        
        protected IList<IMessageReferenceData<TMessageQueue, TJournal>> _referenceData;

        public SmartProxyBase(
            ISmartProxyRequestSmartProxyConsumer<TMessageQueue, TMessage, TJournal> requestSmartProxyConsumer,
            ISmartProxyReplySmartProxyConsumer<TMessageQueue, TMessage, TJournal> replySmartProxyConsumer)
        {
            _referenceData = new List<IMessageReferenceData<TMessageQueue, TJournal>>();

            _requestSmartProxyConsumer = requestSmartProxyConsumer;
            _replySmartProxyConsumer = replySmartProxyConsumer;

            _requestSmartProxyConsumer.ReferenceData = _referenceData;
            _replySmartProxyConsumer.ReferenceData = _referenceData;
        }

        //Start listening incoming messages
        public virtual void Process()
        {
            _requestSmartProxyConsumer.Process();     
            _replySmartProxyConsumer.Process();
        }

        public virtual void StopProcessing()
        {
            _requestSmartProxyConsumer.StopProcessing();
            _replySmartProxyConsumer.StopProcessing();
        }
    }
}
