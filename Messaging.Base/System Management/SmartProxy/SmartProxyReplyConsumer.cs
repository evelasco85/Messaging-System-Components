using System;
using System.Linq;

namespace Messaging.Base.System_Management.SmartProxy
{
    public abstract class SmartProxyReplyConsumer<TMessageQueue, TMessage, TJournal> : SmartProxyMessageConsumer<TMessageQueue, TMessage, TJournal>, ISmartProxyReplyConsumer<TMessageQueue, TMessage, TJournal>
    {
        public SmartProxyReplyConsumer(
            IMessageReceiver<TMessage> serviceReplyReceiver
            ) : base(serviceReplyReceiver)
        {
        }

        //Received reply from service
        public override void ProcessMessage(TMessage message)       
        {
            /*Retrieve internal journal associated from proxy message*/
            Func<TJournal, bool> internalJournalLookupCondition = GetJournalLookupCondition(message);

            IMessageReferenceData<TMessageQueue, TJournal> matchedReferenceData = this.ReferenceData
                .Where(reference => ((internalJournalLookupCondition != null) && (internalJournalLookupCondition(reference.InternalJournal))))
                .DefaultIfEmpty(null)
                .FirstOrDefault();
            /**/

            if (matchedReferenceData != null)
            {
                AnalyzeMessage(matchedReferenceData, message);

                //Forward reply to original recipient
                SendMessage(matchedReferenceData.ExternalJournal, matchedReferenceData.OriginalReturnAddress, message);
                ReferenceData.Remove(matchedReferenceData);
            }
            else
            {
                Console.WriteLine(this.GetType().Name + "Unrecognized Reply Message");
            }
        }

        public abstract void AnalyzeMessage(IMessageReferenceData<TMessageQueue, TJournal> reference, TMessage replyMessage);
        public abstract Func<TJournal, bool> GetJournalLookupCondition(TMessage message);
        public abstract void SendMessage(TJournal externalJournal, TMessageQueue queue, TMessage message);
    }
}
