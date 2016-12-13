using Messaging.Base.Constructions;

namespace Messaging.Base.System_Management.SmartProxy
{
    public abstract class SmartProxyRequestConsumer<TMessageQueue, TMessage, TJournal> : SmartProxyMessageConsumer<TMessageQueue, TMessage, TJournal>, ISmartProxyRequestConsumer<TMessageQueue, TMessage, TJournal>
    {
        private IRawMessageSender<TMessage> _serviceRequestSender;
        IReturnAddress<TMessage> _serviceReplyReturnAddress;

        public SmartProxyRequestConsumer(
            IMessageReceiver<TMessage> requestReceiver,
            IRawMessageSender<TMessage> serviceRequestSender,
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
            _serviceRequestSender.SendRawMessage(message);
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
