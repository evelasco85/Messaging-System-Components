using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Base.System_Management.SmartProxy
{
    public interface IReplySmartProxyMessageConsumer<TMessageQueue, TMessage, TJournal> : ISmartProxyMessageConsumer<TMessageQueue, TMessage, TJournal>
    {
        Func<TJournal, bool> GetJournalLookupCondition(TMessage message);
    }

    public interface ISmartProxyReplySmartProxyConsumer<TMessageQueue, TMessage, TJournal> : IReplySmartProxyMessageConsumer<TMessageQueue, TMessage, TJournal>
    {
        void AnalyzeMessage(IMessageReferenceData<TMessageQueue, TJournal> reference, TMessage replyMessage);
        void SendMessage(TJournal externalJournal, TMessageQueue queue, TMessage message);
    }
}
