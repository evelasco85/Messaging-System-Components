using CommonObjects;
using Messaging.Base;
using Messaging.Base.Routing;

namespace CreditBureauFailOver
{
    public class FailOverRouter<TMessage> : ContextBasedRouter<TMessage, FailOverRouteEnum>
    {
        public FailOverRouter(
            IMessageReceiver<TMessage> inputQueue,
            params IRawMessageSender<TMessage>[] outputQueues
            )
            : base(inputQueue)
        {
            SetRoute(outputQueues);
        }

        void SetRoute(params IRawMessageSender<TMessage>[] outputQueues)
        {
            this.AddSender(
                (control) => { return control == FailOverRouteEnum.Primary; },      //Invocation condition
                outputQueues[0]
                );

            //Fail-over sender
            this.AddSender(
                (control) => { return control == FailOverRouteEnum.Backup; },              //Invocation condition
                outputQueues[1]
                );
        }
    }
}
