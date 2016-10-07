using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Base.Routing
{
    public interface IMessageRouter
    {
        void SendToRecipent<TMessageQueue, TMessag>(TMessag message,
            IList<IMessageSender<TMessageQueue, TMessag>> recipientList);
    }
}
