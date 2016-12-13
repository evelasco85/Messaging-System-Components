using System;

namespace Messaging.Base.System_Management
{
    public interface ITestMessage<TMessage>
    {
        void SendControlBusStatus<TEntity>(TEntity statusMessage);
        TMessage SendTestMessage<TEntity>(TEntity entity, Action<AssignPriorityDelegate> AssignProperty);
        void ReceiveTestMessageResponse(TMessage message);
    }
}
