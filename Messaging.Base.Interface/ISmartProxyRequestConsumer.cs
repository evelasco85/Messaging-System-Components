using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messaging.Base.System_Management.SmartProxy;

namespace Messaging.Base
{
    public interface ISmartProxyRequestConsumer<TMessageQueue, TMessage, TJournal> : IRequestMessageConsumer<TMessageQueue, TMessage, TJournal>
    {
        void AnalyzeMessage(TMessage message);
        TMessageQueue GetReturnAddress(TMessage message);
    }
}
