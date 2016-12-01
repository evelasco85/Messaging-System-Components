namespace Messaging.Base.System_Management.SmartProxy
{
    public interface ISmartProxyRequestMessageConsumer<TMessageQueue, TMessage, TJournal> : ISmartProxyMessageConsumer<TMessageQueue, TMessage, TJournal>
    {
        TJournal ConstructJournalReference(TMessage message);
    }

    public interface ISmartProxyRequestConsumer<TMessageQueue, TMessage, TJournal> : ISmartProxyRequestMessageConsumer<TMessageQueue, TMessage, TJournal>
    {
        void AnalyzeMessage(TMessage message);
        TMessageQueue GetReturnAddress(TMessage message);
    }
}
