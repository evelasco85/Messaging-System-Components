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
        static FailOverRouter<Message> _failOverRouter;
        static FailOverControlReceiver<Message> _failOverControlReceiver;

        static void Main(string[] args)
        {
            string routerControlQueueName = string.Empty;
            string creditQueueName = string.Empty;
            string primaryCreditQueueName = string.Empty;
            string secondaryCreditQueueName = string.Empty;

            IMessageFormatter creditBureauRequestFormatter = new XmlMessageFormatter(new Type[] {typeof(CreditBureauRequest)});
            IMessageFormatter failOverRouteEnumFormatter = new XmlMessageFormatter(new Type[] {typeof(FailOverRouteEnum)});

            MQOrchestration.GetInstance().CreateClient(
                args[0],
                "MSMQ",
                ToPath(args[1]),
                ToPath(args[2])
                )
                .Register(registration =>
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
                    _failOverRouter = new FailOverRouter<Message>(
                        new MessageReceiverGateway(ToPath(creditQueueName),creditBureauRequestFormatter),
                        new MessageSenderGateway(ToPath(primaryCreditQueueName)),
                        new MessageSenderGateway(ToPath(secondaryCreditQueueName))
                        );
                    _failOverControlReceiver = new FailOverControlReceiver<Message>(
                        new MessageReceiverGateway(
                            ToPath(routerControlQueueName),
                            failOverRouteEnumFormatter
                            ),
                        _failOverRouter,
                        (message => (FailOverRouteEnum)message.Body)
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
                    if (_failOverControlReceiver != null)
                    {
                        _failOverControlReceiver.StartProcessing();
                        Console.WriteLine("Starting Application!");
                    }
                },
                () =>
                {
                    //Stop
                    if (_failOverControlReceiver != null)
                    {
                        _failOverControlReceiver.StopProcessing();
                        Console.WriteLine("Stopping Application!");
                    }
                })
                .Process();

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
