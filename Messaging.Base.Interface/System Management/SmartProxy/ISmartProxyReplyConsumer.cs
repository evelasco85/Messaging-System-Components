using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Base.System_Management.SmartProxy
{
    public interface ISmartProxyReplyConsumer<TMessageQueue, TMessage, TJournal> : IReplyMessageConsumer<TMessageQueue, TMessage, TJournal>
    {
        void AnalyzeMessage(IMessageReferenceData<TMessageQueue, TJournal> reference, TMessage replyMessage);
        void SendMessage(TJournal externalJournal, TMessageQueue queue, TMessage message);
    }
}
