using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Base.System_Management.SmartProxy
{
    public interface ISmartProxyReplyConsumer<TMessageQueue, TMessage> : IMessageConsumer<TMessageQueue, TMessage>
    {
        void AnalyzeMessage(MessageReferenceData<TMessageQueue, TMessage> referenceData, TMessage replyMessage);
    }

    public abstract class SmartProxyReplyConsumer<TMessageQueue, TMessage> : MessageConsumer<TMessageQueue, TMessage>, ISmartProxyReplyConsumer<TMessageQueue, TMessage>
    {
        public SmartProxyReplyConsumer(
            IMessageReceiver<TMessageQueue, TMessage> reply
            ) : base(reply)
        {
        }

        public override void ProcessMessage(TMessage message)
        {
            MessageReferenceData<TMessageQueue, TMessage> referenceData = GetReferenceData(message);
            MessageReferenceData<TMessageQueue, TMessage> matchedReferenceData = ReferenceData
                .Where(reference => reference.CorrID == referenceData.CorrID)
                .DefaultIfEmpty(null)
                .FirstOrDefault();

            if (matchedReferenceData != null)
            {
                AnalyzeMessage(matchedReferenceData, message);
                matchedReferenceData.ReplyQueue.Send(message);

                ReferenceData.Remove(matchedReferenceData);
            }
            else
            {
                Console.WriteLine(this.GetType().Name + "Unrecognized Reply Message");
            }
        }

        public abstract void AnalyzeMessage(MessageReferenceData<TMessageQueue, TMessage> referenceData, TMessage replyMessage);
    }
}
