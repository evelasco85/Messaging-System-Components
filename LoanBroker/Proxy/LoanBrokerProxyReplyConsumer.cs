using System;
using System.Collections;
using System.Messaging;
using Messaging.Base;
using Messaging.Base.System_Management.SmartProxy;

namespace LoanBroker
{
    public class LoanBrokerProxySmartProxyReplyConsumer : SmartProxySmartProxyReplyConsumer<MessageQueue, Message, ProxyJournal>
    {
        public class LoanBrokerProxyInfo
        {
            public string Content { get; set; }
            public string Detail { get; set; }
        }

        private ArrayList _queueStats;
        private ArrayList _performanceStats;
        private IMessageSender<Message> _controlBus;

        public LoanBrokerProxySmartProxyReplyConsumer
            (
            IMessageReceiver<Message> serviceReplyReceiver,
            ArrayList queueStats,
            ArrayList performanceStats,
            IMessageSender<Message> controlBus
            ) : base(serviceReplyReceiver)
        {
            _queueStats = queueStats;
            _performanceStats = performanceStats;
            _controlBus = controlBus;
        }

        public override void AnalyzeMessage(IMessageReferenceData<MessageQueue, ProxyJournal> reference, Message replyMessage)
        {
            TimeSpan duration = DateTime.Now - replyMessage.SentTime;
            int outstandingReferenceDataCount = this.ReferenceData.Count - 1;

            _performanceStats.Add(duration.TotalSeconds);
            _queueStats.Add(outstandingReferenceDataCount);

            Console.WriteLine("--->service reply processing time: {0:f}", duration.TotalSeconds);

            if (_controlBus != null)
            {
                //_controlBus.Send(duration.TotalSeconds.ToString() + "," + this.ReferenceData.Count);

                LoanBrokerProxyInfo info = new LoanBrokerProxyInfo
                {
                    Detail =
                        string.Format("Reply Duration [{0}] | Outstanding Request [{1}]", duration.TotalSeconds,
                            outstandingReferenceDataCount)
                };

                _controlBus.Send(new Message(info));
            }
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
