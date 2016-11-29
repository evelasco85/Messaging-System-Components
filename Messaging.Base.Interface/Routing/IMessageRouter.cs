using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Base.Routing
{
    public interface IMessageRouter
    {
        void SendToRecipent<TMessage>(TMessage message,
            IList<IMessageSender<TMessage>> recipientList);
    }
}
