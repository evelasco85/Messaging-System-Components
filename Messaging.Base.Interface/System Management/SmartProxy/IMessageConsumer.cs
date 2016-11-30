using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Base.System_Management.SmartProxy
{
    public interface IMessageConsumer
    {
        bool ProcessStarted { get; }
        bool Process();
        void StopProcessing();
    }

    public interface IMessageConsumer<TMessage> : IMessageConsumer
    {
        void ProcessMessage(TMessage message);
    }
}
