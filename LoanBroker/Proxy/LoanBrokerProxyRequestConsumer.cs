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

namespace LoanBroker
{
    public class LoanBrokerProxyRequestConsumer : SmartProxyRequestConsumer<MessageQueue, Message, ProxyJournal>
    {
        ArrayList _queueStats;

        public LoanBrokerProxyRequestConsumer(
            IMessageReceiver<MessageQueue, Message> requestReceiver,
            IMessageSender<MessageQueue, Message> serviceRequestSender,
            IReturnAddress<Message> returnAddress,
            IMessageSender<MessageQueue, Message> output,
            ArrayList queueStats
            )
            : base(requestReceiver, serviceRequestSender, returnAddress, output)

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
