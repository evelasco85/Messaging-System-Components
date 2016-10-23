using Messaging.Base.System_Management.SmartProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Base.Routing
{
    public interface IContextBasedRouter<TMessageQueue, TMessage, TInput> : IMessageConsumer<TMessageQueue, TMessage>
    {
        IContextBasedRouter<TMessageQueue, TMessage, TInput> AddSender(Func<TInput, bool> triggerFunction, IMessageSender<TMessageQueue, TMessage> destination);
        void SwitchDestination(TInput inputToVerify);
    }
}
