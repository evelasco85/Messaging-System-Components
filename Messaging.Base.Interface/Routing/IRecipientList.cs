using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Base.Routing
{
    public interface IRecipientList<TBaseEntity, TMessageQueue, TMessage>
    {
        void AddRecipient(TBaseEntity baseEntity);
        void AddRecipient(params TBaseEntity[] recipients);
        IList<IMessageSender<TMessageQueue, TMessage>> GetRecipients(Func<TBaseEntity, bool> recipientCondition);
        IList<IMessageSender<TMessageQueue, TMessage>> GetRecipients();
    }
}
