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

    public class SmartProxyBase<TMessageQueue, TMessage> : ISmartProxyBase
    {
        ISmartProxyRequestConsumer<TMessageQueue, TMessage> _requestConsumer;
        ISmartProxyReplyConsumer<TMessageQueue, TMessage> _replyConsumer;
        IList<MessageReferenceData<TMessageQueue, TMessage>> _referenceData;

        public SmartProxyBase(
            ISmartProxyRequestConsumer<TMessageQueue, TMessage> requestConsumer,
            ISmartProxyReplyConsumer<TMessageQueue, TMessage> replyConsumer)
        {
            _referenceData = new SynchronizedCollection<MessageReferenceData<TMessageQueue, TMessage>>();

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
