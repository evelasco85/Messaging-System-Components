using CommonObjects;
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
        FailOverRouter _failOverRouter;
        public FailOverControlReceiver(IMessageReceiver<MessageQueue, Message> inputQueue, FailOverRouter failOverRouter)
            : base(inputQueue)
        {
            _failOverRouter = failOverRouter;
        }

        public override void ProcessMessage(Message message)
        {
            FailOverRouteEnum route = (FailOverRouteEnum)message.Body;

            if (_failOverRouter == null)
                return;

            if (route != FailOverRouteEnum.Standby)
            {
                SetRoute(route);

                if (!_failOverRouter.ProcessStarted)
                    _failOverRouter.Process();
            }
            else
                Console.WriteLine("Routing credit request on-standby");
        }

        void SetRoute(FailOverRouteEnum route)
        {
            _failOverRouter.SwitchDestination(route);
            Console.WriteLine("Route Set: {0}", route.ToString());
        }
    }
}
