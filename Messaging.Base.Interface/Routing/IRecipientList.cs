using System;
using System.Collections.Generic;

namespace Messaging.Base.Routing
{
    public interface IRecipientList<TBaseEntity, TMessage>
    {
        void AddRecipient(TBaseEntity baseEntity);
        void AddRecipient(params TBaseEntity[] recipients);
        IList<IMessageSender<TMessage>> GetRecipients(Func<TBaseEntity, bool> recipientCondition);
        IList<IMessageSender<TMessage>> GetRecipients();
    }
}
