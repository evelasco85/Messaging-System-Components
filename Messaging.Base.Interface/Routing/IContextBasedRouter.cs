using Messaging.Base.System_Management.SmartProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Base.Routing
{
    public interface IContextBasedRouter<TMessage, TInput> : IMessageConsumer<TMessage>
    {
        bool DestinationIsSet { get; }
        IContextBasedRouter<TMessage, TInput> AddSender(Func<TInput, bool> triggerFunction, IMessageSender<TMessage> destination);
        void SwitchDestination(TInput inputToVerify);
    }
}
