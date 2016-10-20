using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messaging.Base.System_Management.SmartProxy;

namespace Messaging.Base.System_Management.SmartProxy
{
    public interface IRequestSmartProxyMessageConsumer<TMessageQueue, TMessage, TJournal> : ISmartProxyMessageConsumer<TMessageQueue, TMessage, TJournal>
    {
        TJournal ConstructJournalReference(TMessage message);
    }

    public interface ISmartProxyRequestSmartProxyConsumer<TMessageQueue, TMessage, TJournal> : IRequestSmartProxyMessageConsumer<TMessageQueue, TMessage, TJournal>
    {
        void AnalyzeMessage(TMessage message);
        TMessageQueue GetReturnAddress(TMessage message);
    }
}
