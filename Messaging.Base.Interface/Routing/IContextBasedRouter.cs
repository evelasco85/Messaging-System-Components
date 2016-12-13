using Messaging.Base.System_Management.SmartProxy;
using System;

namespace Messaging.Base.Routing
{
    public interface IContextBasedRouter<TMessage, TInput> : IMessageConsumer<TMessage>
    {
        bool DestinationIsSet { get; }
        IContextBasedRouter<TMessage, TInput> AddSender(Func<TInput, bool> triggerFunction, IRawMessageSender<TMessage> destination);
        void SwitchDestination(TInput inputToVerify);
    }
}
