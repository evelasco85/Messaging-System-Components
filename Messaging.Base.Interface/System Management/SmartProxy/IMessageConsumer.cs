using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Base.System_Management.SmartProxy
{
    public interface IMessageConsumer<TMessageQueue, TMessage>
    {
        void Process();
        void ProcessMessage(TMessage message);
    }
}
