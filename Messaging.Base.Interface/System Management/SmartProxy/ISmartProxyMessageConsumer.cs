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

    public interface ISmartProxyMessageConsumer<TMessageQueue, TMessage, TJournal> : IMessageConsumer<TMessageQueue, TMessage>
    {
        IList<IMessageReferenceData<TMessageQueue, TJournal>> ReferenceData { get; set; }
    }
}
