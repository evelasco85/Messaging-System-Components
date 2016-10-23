using CommonObjects;
using MessageGateway;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace CreditBureauFailOver
{
    class Program
    {
        static FailOverRouter _failOverRouter;
        static void Main(string[] args)
        {
            string creditQueueName = ToPath(args[0]);
            string primaryCreditQueueName = ToPath(args[1]);
            string secondaryCreditQueueName = ToPath(args[2]);

            _failOverRouter = new FailOverRouter(new MessageReceiverGateway(creditQueueName, GetFormatter()));

            _failOverRouter.SetRoute(primaryCreditQueueName, secondaryCreditQueueName);
            _failOverRouter.SwitchDestination(0);
            _failOverRouter.Process();

            Console.WriteLine();
            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }

        private static String ToPath(String arg)
        {
            return ".\\private$\\" + arg;
        }

        public static IMessageFormatter GetFormatter()
        {
            return new XmlMessageFormatter(new Type[] { typeof(CreditBureauRequest) });
        }
    }
}
