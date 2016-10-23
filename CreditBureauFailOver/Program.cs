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
        static FailOverControlReceiver _failOverControlReceiver;

        static void Main(string[] args)
        {
            if (args.Count() == 4)
            {
                string routerControlQueueName = ToPath(args[0]);
                string creditQueueName = ToPath(args[1]);
                string primaryCreditQueueName = ToPath(args[2]);
                string secondaryCreditQueueName = ToPath(args[3]);

                _failOverRouter = new FailOverRouter(
                    primaryCreditQueueName, secondaryCreditQueueName,
                    new MessageReceiverGateway(creditQueueName, new XmlMessageFormatter(new Type[] { typeof(CreditBureauRequest) }))
                    );
                _failOverControlReceiver = new FailOverControlReceiver(
                    new MessageReceiverGateway(routerControlQueueName, new XmlMessageFormatter(new Type[] { typeof(FailOverRouteEnum) })),
                    _failOverRouter
                    );

                _failOverControlReceiver.Process();
            }
            else
                Console.WriteLine("Usage: <>");

            Console.WriteLine();
            Console.WriteLine("Press Enter to exit...");
            Console.WriteLine();
            Console.ReadLine();
        }

        private static String ToPath(String arg)
        {
            return ".\\private$\\" + arg;
        }
    }
}
