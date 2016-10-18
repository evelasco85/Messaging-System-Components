using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Base.System_Management.SmartProxy
{
    public interface ISmartProxyReplyConsumer<TMessageQueue, TMessage, TJournal> : IReplyMessageConsumer<TMessageQueue, TMessage, TJournal>
    {
        void AnalyzeMessage(MessageReferenceData<TMessageQueue, TJournal> reference, TMessage replyMessage);
        void SendMessage(TJournal externalJournal, TMessageQueue queue, TMessage message);
    }

    public abstract class SmartProxyReplyConsumer<TMessageQueue, TMessage, TJournal> : MessageConsumer<TMessageQueue, TMessage, TJournal>, ISmartProxyReplyConsumer<TMessageQueue, TMessage, TJournal>
    {
        public SmartProxyReplyConsumer(
            IMessageReceiver<TMessageQueue, TMessage> serviceReplyReceiver
            ) : base(serviceReplyReceiver)
        {
        }

        //Received reply from service
        public override void ProcessMessage(TMessage message)       
        {
            /*Retrieve internal journal associated from proxy message*/
            Func<TJournal, bool> internalJournalLookupCondition = GetJournalLookupCondition(message);

            MessageReferenceData<TMessageQueue, TJournal> matchedReferenceData = this.ReferenceData
                .Where(reference => internalJournalLookupCondition(reference.InternalJournal))
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

        public abstract void AnalyzeMessage(MessageReferenceData<TMessageQueue, TJournal> reference, TMessage replyMessage);
        public abstract Func<TJournal, bool> GetJournalLookupCondition(TMessage message);
        public abstract void SendMessage(TJournal externalJournal, TMessageQueue queue, TMessage message);
    }
}
