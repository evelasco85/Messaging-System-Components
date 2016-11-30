using Messaging.Base.Constructions;

namespace Messaging.Base.System_Management.SmartProxy
{
    public abstract class SmartProxySmartProxyRequestConsumer<TMessageQueue, TMessage, TJournal> : SmartProxySmartProxyMessageConsumer<TMessageQueue, TMessage, TJournal>, ISmartProxyRequestSmartProxyConsumer<TMessageQueue, TMessage, TJournal>
    {
        private IMessageSender<TMessage> _serviceRequestSender;
        IReturnAddress<TMessage> _serviceReplyReturnAddress;

        public SmartProxySmartProxyRequestConsumer(
            IMessageReceiver<TMessage> requestReceiver,
            IMessageSender<TMessage> serviceRequestSender,
            IReturnAddress<TMessage> serviceReplyReturnAddress
            ) : base(requestReceiver)
        {
            _serviceRequestSender = serviceRequestSender;
            _serviceReplyReturnAddress = serviceReplyReturnAddress;
        }

        //Received message from client
        public override void ProcessMessage(TMessage message)               
        {
            /*Record journal information from incoming requests*/
            TMessageQueue originalReturnAddress = GetReturnAddress(message);
            TJournal externalJournal = ConstructJournalReference(message);
            /**/

            /*Forward message to destination service*/
            _serviceReplyReturnAddress.SetMessageReturnAddress(ref message);        //Ensures that destination replies to 'return address'
            _serviceRequestSender.Send(message);
            /**/

            /*Store proxy message journal(internal and external)*/
            MessageReferenceData<TMessageQueue, TJournal> refData = new MessageReferenceData<TMessageQueue, TJournal>
            {
                InternalJournal = ConstructJournalReference(message),
                ExternalJournal = externalJournal,
                OriginalReturnAddress = originalReturnAddress
            };

            ReferenceData.Add(refData);
            AnalyzeMessage(message);
            /**/
        }

        public abstract void AnalyzeMessage(TMessage message);
        public abstract TJournal ConstructJournalReference(TMessage message);
        public abstract TMessageQueue GetReturnAddress(TMessage message);
    }
}
