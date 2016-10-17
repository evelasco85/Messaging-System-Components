using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Base.System_Management.SmartProxy
{
    public interface ISmartProxyRequestConsumer<TMessageQueue, TMessage, TJournal> : IRequestMessageConsumer<TMessageQueue, TMessage, TJournal>
    {
        void AnalyzeMessage(TMessage message);
    }

    public abstract class SmartProxyRequestConsumer<TMessageQueue, TMessage, TJournal> : MessageConsumer<TMessageQueue, TMessage, TJournal>, ISmartProxyRequestConsumer<TMessageQueue, TMessage, TJournal>
    {
        private IMessageSender<TMessageQueue, TMessage> _serviceRequestSender;

        public SmartProxyRequestConsumer(
            IMessageReceiver<TMessageQueue, TMessage> requestReceiver,
            IMessageSender<TMessageQueue, TMessage> serviceRequestSender
            ) : base(requestReceiver)
        {
            _serviceRequestSender = serviceRequestSender;
        }

        public override void ProcessMessage(TMessage message)               //Received message from client
        {
            _serviceRequestSender.Send(message);                            //Forward message to destination(service)
            ReferenceData.Add(ConstructJournalReference(message));          //store message reference
            AnalyzeMessage(message);
        }

        public abstract void AnalyzeMessage(TMessage message);
        
    }
}
