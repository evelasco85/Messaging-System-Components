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
    public class LoanBrokerProxySmartProxyRequestConsumer : SmartProxySmartProxyRequestConsumer<MessageQueue, Message, ProxyJournal>
    {
        ArrayList _queueStats;

        public LoanBrokerProxySmartProxyRequestConsumer(
            IMessageReceiver<MessageQueue, Message> requestMonitorReceiver,
            IMessageSender<MessageQueue, Message> serviceRequestSender,
            IReturnAddress<Message> serviceReplyReturnAddress,
            ArrayList queueStats
            )
            : base(requestMonitorReceiver, serviceRequestSender, serviceReplyReturnAddress)
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
