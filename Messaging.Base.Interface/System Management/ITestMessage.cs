using System.Collections.Generic;

namespace Messaging.Base.System_Management
{
    public interface ITestMessage<TMessage>
    {
        void SendControlBusStatus<TEntity>(TEntity statusMessage);
        void SendControlBusStatus(TMessage statusMessage);
        TMessage SendTestMessage<TEntity>(TEntity entity, IList<SenderProperty> properties);
        void ReceiveTestMessageResponse(TMessage message);
    }
}
