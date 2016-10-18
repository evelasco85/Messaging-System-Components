using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;
using Messaging.Base;
using Messaging.Base.System_Management.SmartProxy;

namespace LoanBroker
{
    public class LoanBrokerProxyReplyConsumer : SmartProxyReplyConsumer<MessageQueue, Message, ProxyJournal>
    {
        private ArrayList _queueStats;
        private ArrayList _performanceStats;

        public LoanBrokerProxyReplyConsumer
            (
            IMessageReceiver<MessageQueue, Message> serviceReplyReceiver,
            ArrayList queueStats,
            ArrayList performanceStats
            ) : base(serviceReplyReceiver)
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

        public override Func<ProxyJournal, bool> GetJournalLookupCondition(Message message)
        {
            return (journal) =>
            {
                return ((journal.CorrelationId == message.CorrelationId) &&
                        (journal.AppSpecific == message.AppSpecific))
                    ;
            };
        }

        public override void SendMessage(MessageQueue queue, Message message)
        {
            queue.Send(message);
        }
    }
}
