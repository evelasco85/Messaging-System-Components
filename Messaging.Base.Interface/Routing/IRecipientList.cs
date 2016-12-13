using System;
using System.Collections.Generic;

namespace Messaging.Base.Routing
{
    public interface IRecipientList<TBaseEntity, TMessage>
    {
        void AddRecipient(TBaseEntity baseEntity);
        void AddRecipient(params TBaseEntity[] recipients);
        IList<IRawMessageSender<TMessage>> GetRecipients(Func<TBaseEntity, bool> recipientCondition);
        IList<IRawMessageSender<TMessage>> GetRecipients();
    }
}
