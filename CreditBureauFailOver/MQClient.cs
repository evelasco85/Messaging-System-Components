using System;
using System.Messaging;
using CommonObjects;
using MsmqGateway.Core;
using Messaging.Orchestration.Shared.Services;
using MsmqGateway;

namespace CreditBureauFailOver
{
    class MQClient
    {
        public static IClientService GetClientService(ClientInstance<Message> instance, String[] args)
        {
            string routerControlQueueName = string.Empty;
            string creditQueueName = string.Empty;
            string primaryCreditQueueName = string.Empty;
            string secondaryCreditQueueName = string.Empty;

            return MQOrchestration.GetInstance().CreateClient(
                args[0],
                "MSMQ",
                ToPath(args[1]),
                ToPath(args[2])
                )
                .Register(registration =>
                {
                    //Server parameter requests
                    registration.RegisterRequiredServerParameters("routerControlQueueName",
                        (value) => routerControlQueueName = (string) value);
                    registration.RegisterRequiredServerParameters("creditQueueName",
                        (value) => creditQueueName = (string) value);
                    registration.RegisterRequiredServerParameters("primaryCreditQueueName",
                        (value) => primaryCreditQueueName = (string) value);
                    registration.RegisterRequiredServerParameters("secondaryCreditQueueName",
                        (value) => secondaryCreditQueueName = (string) value);
                },
                    errorMessage =>
                    {
                        //Invalid registration
                    },
                    () =>
                    {
                        MessageReceiverGateway<FailOverRouteEnum> failOverRouteReceiver =
                            new MessageReceiverGateway<FailOverRouteEnum>(ToPath(routerControlQueueName));

                        instance.SetupFailOver(
                            new FailOverRouter<Message>(
                                new MessageReceiverGateway<CreditBureauRequest>(ToPath(creditQueueName)),
                                new MessageSenderGateway(ToPath(primaryCreditQueueName)),
                                new MessageSenderGateway(ToPath(secondaryCreditQueueName))
                                ),
                            new FailOverControlReceiver<Message>(failOverRouteReceiver,
                                failOverRouteReceiver.CanonicalDataModel.GetEntity
                                )
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
                        instance.Start();
                    },
                    () =>
                    {
                        //Stop
                        instance.Stop();
                    });
        }

        static String ToPath(String arg)
        {
            return ".\\private$\\" + arg;
        }
    }
}
