using Messaging.Base;
using Messaging.Base.System_Management.SmartProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace CreditBureauFailOver
{
    public class FailOverControlReceiver : MessageConsumer<MessageQueue, Message>
    {
        public FailOverControlReceiver(IMessageReceiver<MessageQueue, Message> inputQueue)
            : base(inputQueue)
        {

        }

        public override void ProcessMessage(Message message)
        {
            throw new NotImplementedException();
        }
    }
}
