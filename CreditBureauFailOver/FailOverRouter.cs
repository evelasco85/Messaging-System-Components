using CommonObjects;
using MessageGateway;
using Messaging.Base;
using Messaging.Base.Routing;
using System.Messaging;

namespace CreditBureauFailOver
{
    public class FailOverRouter : ContextBasedRouter<Message, FailOverRouteEnum>
    {
        public FailOverRouter(string primaryOutputQueue, string backupOutputQueue,
            IMessageReceiver<Message> inputQueue)
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
