using System;
using Messaging.Base.Constructions;

namespace Messaging.Base
{
    public delegate void AssignSenderPropertyDelegate(string name, object value);

    public interface IMessageSender
    {
        string QueueName { get; }
        void SetupSender();
    }

    public interface IMessageSender<TMessage> : IMessageSender
    {
        IReturnAddress<TMessage> AsReturnAddress();
        TMessage Send(TMessage message);
        TMessage Send<TEntity>(TEntity message);
        TMessage Send<TEntity>(TEntity entity, Action<AssignSenderPropertyDelegate> AssignProperty);
        TMessage Send<TEntity>(TEntity entity, IReturnAddress<TMessage> returnAddress);
        TMessage Send<TEntity>(TEntity entity, IReturnAddress<TMessage> returnAddress, Action<AssignSenderPropertyDelegate> AssignProperty);
    }

    public interface IMessageSender<TMessageQueue, TMessage> : IMessageCore<TMessageQueue>, IMessageSender<TMessage>
    {
    }
}
