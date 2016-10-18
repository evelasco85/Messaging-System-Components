using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Base.System_Management.SmartProxy
{
    public interface IMessageReferenceData<TMessageQueue, TJournal>
    {
        TJournal InternalJournal { get; set; }       //Proxy related journal
        TJournal ExternalJournal { get; set; }       //External system journal
        TMessageQueue OriginalReturnAddress { get; set; }
    }

    public interface IMessageConsumer<TMessageQueue, TMessage, TJournal>
    {
        IList<IMessageReferenceData<TMessageQueue, TJournal>> ReferenceData { get; set; }

        void Process();
        void ProcessMessage(TMessage message);
    }

    public interface IRequestMessageConsumer<TMessageQueue, TMessage, TJournal> : IMessageConsumer<TMessageQueue, TMessage, TJournal>
    {
        TJournal ConstructJournalReference(TMessage message);
    }

    public interface IReplyMessageConsumer<TMessageQueue, TMessage, TJournal> : IMessageConsumer<TMessageQueue, TMessage, TJournal>
    {
        Func<TJournal, bool> GetJournalLookupCondition(TMessage message);
    }
}
