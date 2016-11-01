using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Base.System_Management.SmartProxy
{
    public interface IMessageConsumer<TMessageQueue, TMessage>
    {
        bool ProcessStarted { get; }
        bool Process();
        void StopProcessing();
        void ProcessMessage(TMessage message);
    }
}
