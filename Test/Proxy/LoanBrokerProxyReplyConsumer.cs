using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;
using Messaging.Base;
using Messaging.Base.System_Management.SmartProxy;

namespace Test.Proxy
{
    public class LoanBrokerProxyReplyConsumer : SmartProxyReplyConsumer<MessageQueue, Message, ProxyJournal>
    {
        private ArrayList _queueStats;
        private ArrayList _performanceStats;
        public LoanBrokerProxyReplyConsumer
            (
            IMessageReceiver<MessageQueue, Message> replyReceiver,
            ArrayList queueStats,
            ArrayList performanceStats
            ) : base(replyReceiver)
        {
            _queueStats = queueStats;
            _performanceStats = performanceStats;
        }

        public override void AnalyzeMessage(IList<MessageReferenceData<MessageQueue, Message, ProxyJournal>> references, Message replyMessage)
        {
            TimeSpan duration = DateTime.Now - replyMessage.SentTime;

            _performanceStats.Add(duration.TotalSeconds);
            _queueStats.Add(references.Count);
        }

        public override MessageReferenceData<MessageQueue, Message, ProxyJournal> GetJournalReference(IList<MessageReferenceData<MessageQueue, Message, ProxyJournal>> references, Message message)
        {
            return references
                .Where(reference => 
                    (reference.Journal.CorrelationId == message.CorrelationId) && 
                    (reference.Journal.AppSpecific == message.AppSpecific))
                .DefaultIfEmpty(null)
                .FirstOrDefault();
        }
    }
}
