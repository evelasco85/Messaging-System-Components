using System;
using System.Collections.Generic;

namespace Messaging.Base.System_Management
{
    public interface ITestMessage<TMessage>
    {
        void SendControlBusStatus<TEntity>(TEntity statusMessage);
        TMessage SendTestMessage<TEntity>(TEntity entity, Action<AssignSenderPropertyDelegate> AssignProperty);
        void ReceiveTestMessageResponse(TMessage message);
    }
}
