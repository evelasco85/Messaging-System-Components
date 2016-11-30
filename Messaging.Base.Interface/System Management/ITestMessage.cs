using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Base.System_Management
{
    public interface ITestMessage<TMessage>
    {
        void SendControlBusStatus<TEntity>(TEntity statusMessage);
        void SendControlBusStatus(TMessage statusMessage);
        void SendTestMessage(TMessage message);
        void ReceiveTestMessageResponse(TMessage message);
    }
}
