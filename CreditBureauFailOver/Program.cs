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
            if (args.Count() == 4)
            {
                string routerControlQueueName = ToPath(args[0]);
                string creditQueueName = ToPath(args[1]);
                string primaryCreditQueueName = ToPath(args[2]);
                string secondaryCreditQueueName = ToPath(args[3]);

                _failOverRouter = new FailOverRouter(new MessageReceiverGateway(creditQueueName, GetReceivedMessageFormatter()));

                _failOverRouter.SetRoute(primaryCreditQueueName, secondaryCreditQueueName);
                _failOverRouter.SwitchDestination(FailOverEnum.Primary);
                _failOverRouter.Process();
            }
            else
                Console.WriteLine("Usage: <>");

            Console.WriteLine();
            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }

        private static String ToPath(String arg)
        {
            return ".\\private$\\" + arg;
        }

        public static IMessageFormatter GetReceivedMessageFormatter()
        {
            return new XmlMessageFormatter(new Type[] { typeof(CreditBureauRequest) });
        }
    }
}
