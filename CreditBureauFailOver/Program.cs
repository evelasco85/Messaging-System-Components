using CommonObjects;
using MessageGateway;
using System;
using System.Messaging;
using Messaging.Orchestration.Shared.Services;
using MsmqGateway;

namespace CreditBureauFailOver
{
    class Program
    {
        static FailOverRouter _failOverRouter;
        static FailOverControlReceiver _failOverControlReceiver;

        static void Main(string[] args)
        {
            string routerControlQueueName = string.Empty;
            string creditQueueName = string.Empty;
            string primaryCreditQueueName = string.Empty;
            string secondaryCreditQueueName = string.Empty;

            IClientService client = MQOrchestration.GetInstance().CreateClient(
                args[0],
                "MSMQ",
                ToPath(args[1]),
                ToPath(args[2])
                );

            client.Register(registration =>
            {
                //Server parameter requests
                registration.RegisterRequiredServerParameters("routerControlQueueName", (value) => routerControlQueueName = (string)value);
                registration.RegisterRequiredServerParameters("creditQueueName", (value) => creditQueueName = (string)value);
                registration.RegisterRequiredServerParameters("primaryCreditQueueName", (value) => primaryCreditQueueName = (string)value);
                registration.RegisterRequiredServerParameters("secondaryCreditQueueName", (value) => secondaryCreditQueueName = (string)value);
            },
                errorMessage =>
                {
                    //Invalid registration
                },
                () =>
                {
                    //Client parameter setup completed
                    _failOverRouter = new FailOverRouter(
                        ToPath(primaryCreditQueueName), ToPath(secondaryCreditQueueName),
                        new MessageReceiverGateway(
                            ToPath(creditQueueName),
                            new XmlMessageFormatter(new Type[] {typeof(CreditBureauRequest)})
                            )
                        );
                    _failOverControlReceiver = new FailOverControlReceiver(
                        new MessageReceiverGateway(
                            ToPath(routerControlQueueName),
                            new XmlMessageFormatter(new Type[] {typeof(FailOverRouteEnum)})
                            ),
                        _failOverRouter
                        );

                    Console.WriteLine("Configurations ok!");
                },
                () =>
                {
                    //Stand-by
                    Console.WriteLine("Stand-by Application!");
                },
                () =>
                {
                    //Start
                    _failOverControlReceiver.Process();
                    Console.WriteLine("Starting Application!");
                },
                () =>
                {
                    //Stop
                    _failOverControlReceiver.StopProcessing();
                    Console.WriteLine("Stopping Application!");
                });

                client.Process();

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
