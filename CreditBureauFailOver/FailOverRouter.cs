using CommonObjects;
using MessageGateway;
using Messaging.Base;
using Messaging.Base.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace CreditBureauFailOver
{
    public class FailOverRouter : ContextBasedRouter<MessageQueue, Message, FailOverEnum>
    {
        public FailOverRouter(IMessageReceiver<MessageQueue, Message> inputQueue)
            : base(inputQueue)
        {
        }

        public void SetRoute(string primaryOutputQueue, string backupOutputQueue)
        {
            this.AddSender(
                (control) => { return control == FailOverEnum.Primary; },      //Invocation condition
                new MessageSenderGateway(primaryOutputQueue)
                );

            //Fail-over sender
            this.AddSender(
                (control) => { return true; },              //Invocation condition
                new MessageSenderGateway(backupOutputQueue)
                );
        }
    }
}
