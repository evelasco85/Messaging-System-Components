using System.Collections;
using System.Messaging;
using Messaging.Base;
using Messaging.Base.Constructions;
using Messaging.Base.System_Management.SmartProxy;

namespace LoanBroker
{
    public class LoanBrokerProxyRequestConsumer : SmartProxyRequestConsumer<MessageQueue, Message, ProxyJournal>
    {
        ArrayList _queueStats;

        public LoanBrokerProxyRequestConsumer(
            IMessageReceiver<Message> requestReceiver,
            IRawMessageSender<Message> serviceRequestSender,
            IReturnAddress<Message> serviceReplyReturnAddress,
            ArrayList queueStats
            )
            : base(requestReceiver, serviceRequestSender, serviceReplyReturnAddress)
        {
            _queueStats = queueStats;
        }

        public override ProxyJournal ConstructJournalReference(Message message)
        {
            return new ProxyJournal
            {
                AppSpecific = message.AppSpecific,
                CorrelationId = message.Id,
            };
        }

        public override void AnalyzeMessage(Message message)
        {
            _queueStats.Add(this.ReferenceData.Count);
        }

        public override MessageQueue GetReturnAddress(Message message)
        {
            return message.ResponseQueue;
        }
    }
}
