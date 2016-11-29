using System;
using System.Collections.Generic;
using System.Text;
using Messaging.Base.Constructions;

namespace Messaging.Base
{
    public interface IMessageSender
    {
        void SetupSender();
    }

    public interface IMessageSender<TMessage>
    {
        IReturnAddress<TMessage> AsReturnAddress();
        void Send(TMessage message);
    }

    public interface IMessageSender<TMessageQueue, TMessage> : IMessageCore<TMessageQueue>, IMessageSender<TMessage>, IMessageSender
    {
    }
}
