using System;
using CommonObjects;
using MessageGateway;
using Messaging.Orchestration.Shared.Services;
using MsmqGateway;

namespace CreditBureau
{
    class MSMQClient
    {
        public static IClientService GetClientService(ClientInstance instance, String[] args)
        {
            String requestQueueName = string.Empty;

            return MQOrchestration.GetInstance().CreateClient(
                args[0],
                "MSMQ",
                ToPath(args[1]),
                ToPath(args[2])
                )
                .Register(registration =>
                {
                    //Server parameter requests
                    registration.RegisterRequiredServerParameters("requestQueueName",
                        (value) => requestQueueName = (string) value);
                },
                    errorMessage =>
                    {
                        //Invalid registration
                    },
                    () =>
                    {
                        //Client parameter setup completed
                        instance.SetupQueueService(
                            new MQRequestReplyService_Synchronous(
                                ToPath(requestQueueName),
                                new SyncProcessMessageDelegate(instance.Proc.ProcessRequestMessage),
                                null,
                                new GetRequestBodyTypeDelegate(() => { return typeof(CreditBureauRequest); })));

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
                        instance.Run();
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
