using System;

namespace Messaging.Base.System_Management.SmartProxy
{
    public interface ISmartProxyReplyMessageConsumer<TMessageQueue, TMessage, TJournal> : ISmartProxyMessageConsumer<TMessageQueue, TMessage, TJournal>
    {
        Func<TJournal, bool> GetJournalLookupCondition(TMessage message);
    }

    public interface ISmartProxyReplyConsumer<TMessageQueue, TMessage, TJournal> : ISmartProxyReplyMessageConsumer<TMessageQueue, TMessage, TJournal>
    {
        void AnalyzeMessage(IMessageReferenceData<TMessageQueue, TJournal> reference, TMessage replyMessage);
        void SendMessage(TJournal externalJournal, TMessageQueue queue, TMessage message);
    }
}
