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
        private IMessageSender<TMessageQueue, TMessage> _replySender;

        public SmartProxyRequestConsumer(
            IMessageReceiver<TMessageQueue, TMessage> requestReceiver,
            IMessageSender<TMessageQueue, TMessage> serviceRequestSender,
            IMessageSender<TMessageQueue, TMessage> replySender
            ) : base(requestReceiver)
        {
            _serviceRequestSender = serviceRequestSender;
            _replySender = replySender;
        }

        public override void ProcessMessage(TMessage message)               //Received message from client
        {
            _serviceRequestSender.Send(message);                            //Forward message to destination(service)

            MessageReferenceData<TMessageQueue, TMessage, TJournal> refData = new MessageReferenceData<TMessageQueue, TMessage, TJournal>
            {
                Journal = ConstructJournalReference(message),       //store message reference
                ReplyAddress = _replySender
            };
                
            ReferenceData.Add(refData);          
            AnalyzeMessage(message);
        }

        public abstract void AnalyzeMessage(TMessage message);
        public abstract TJournal ConstructJournalReference(TMessage message);
    }
}
