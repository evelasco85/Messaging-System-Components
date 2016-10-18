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
        private IMessageSender<MessageQueue, Message> _controlBus;

        public LoanBrokerProxyReplyConsumer
            (
            IMessageReceiver<MessageQueue, Message> serviceReplyReceiver,
            ArrayList queueStats,
            ArrayList performanceStats,
            IMessageSender<MessageQueue, Message> controlBus
            ) : base(serviceReplyReceiver)
        {
            _queueStats = queueStats;
            _performanceStats = performanceStats;
            _controlBus = controlBus;
        }

        public override void AnalyzeMessage(IMessageReferenceData<MessageQueue, ProxyJournal> reference, Message replyMessage)
        {
            TimeSpan duration = DateTime.Now - replyMessage.SentTime;

            _performanceStats.Add(duration.TotalSeconds);
            _queueStats.Add(this.ReferenceData.Count);

            Console.WriteLine("--->service reply processing time: {0:f}", duration.TotalSeconds);

            if(_controlBus != null)
                //_controlBus.Send(duration.TotalSeconds.ToString() + "," + this.ReferenceData.Count);
                _controlBus.GetQueue().Send(duration.TotalSeconds.ToString() + "," + this.ReferenceData.Count);
        }

        public override Func<ProxyJournal, bool> GetJournalLookupCondition(Message message)
        {
            return (journal) =>
            {
                return ((journal.CorrelationId == message.CorrelationId) &&
                        (journal.AppSpecific == message.AppSpecific));
            };
        }

        public override void SendMessage(ProxyJournal externalJournal, MessageQueue queue, Message message)
        {
            message.CorrelationId = externalJournal.CorrelationId;
            message.AppSpecific = externalJournal.AppSpecific;

            queue.Send(message);
        }
    }
}
