using System;
using Messaging.Base.Constructions;

namespace Messaging.Base
{
    public delegate void AssignApplicationIdDelegate(string applicationId);
    public delegate void AssignCorrelationIdDelegate(string correlationId);
    public delegate void AssignPriorityDelegate(object priority);

    public interface IMessageSender
    {
        string QueueName { get; }
        void SetupSender();
    }

    public interface IRawMessageSender<TMessage>
    {
        TMessage SendRawMessage(TMessage message);
    }

    public interface IMessageSender<TMessage> : IMessageSender, IRawMessageSender<TMessage>
    {
        IReturnAddress<TMessage> AsReturnAddress();
        TMessage Send<TEntity>(TEntity message);
        TMessage Send<TEntity>(TEntity entity, Action<AssignApplicationIdDelegate, AssignCorrelationIdDelegate> AssignProperty);
        TMessage Send<TEntity>(TEntity entity, IReturnAddress<TMessage> returnAddress);
        TMessage Send<TEntity>(TEntity entity, IReturnAddress<TMessage> returnAddress, Action<AssignApplicationIdDelegate, AssignCorrelationIdDelegate, AssignPriorityDelegate> AssignProperty);
    }

    public interface IMessageSender<TMessageQueue, TMessage> : IMessageCore<TMessageQueue>, IMessageSender<TMessage>
    {
    }
}
