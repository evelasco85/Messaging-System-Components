using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;
using Messaging.Base;
using Messaging.Base.Constructions;
using Messaging.Base.System_Management.SmartProxy;
using MsmqGateway;

namespace Test.Proxy
{
    public class LoanBrokerRequestConsumer : SmartProxyRequestConsumer<MessageQueue, Message, ProxyJournal>
    {
        ArrayList _queueStats;

        public LoanBrokerRequestConsumer(
            IMessageReceiver<MessageQueue, Message> requestReceiver,
            IMessageSender<MessageQueue, Message> serviceRequestSender,
            IMessageSender<MessageQueue, Message> replySender,
            ArrayList queueStats
            )
            : base(requestReceiver, serviceRequestSender, replySender)

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
    }
}
