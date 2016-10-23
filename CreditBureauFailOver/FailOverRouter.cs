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
    public class FailOverRouter : ContextBasedRouter<MessageQueue, Message, FailOverRouteEnum>
    {
        public FailOverRouter(string primaryOutputQueue, string backupOutputQueue,
            IMessageReceiver<MessageQueue, Message> inputQueue)
            : base(inputQueue)
        {
            SetRoute(primaryOutputQueue, backupOutputQueue);
        }

        void SetRoute(string primaryOutputQueue, string backupOutputQueue)
        {
            this.AddSender(
                (control) => { return control == FailOverRouteEnum.Primary; },      //Invocation condition
                new MessageSenderGateway(primaryOutputQueue)
                );

            //Fail-over sender
            this.AddSender(
                (control) => { return control == FailOverRouteEnum.Backup; },              //Invocation condition
                new MessageSenderGateway(backupOutputQueue)
                );
        }
    }
}
