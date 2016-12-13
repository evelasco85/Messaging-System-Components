using System.Collections.Generic;

namespace Messaging.Base.Routing
{
    public interface IMessageRouter
    {
        void SendToRecipent<TMessage>(TMessage message,
            IList<IRawMessageSender<TMessage>> recipientList);
    }
}
