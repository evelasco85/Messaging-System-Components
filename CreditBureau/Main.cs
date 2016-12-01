using System;
using CommonObjects;
using MessageGateway;
using MsmqGateway;

namespace CreditBureau
{
    public class Run
    {
        public static void Main(String[] args)
        {
            String requestQueueName = string.Empty;
            ClientInstance instance = new ClientInstance();

            MQOrchestration.GetInstance().CreateClient(
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
                    })
                .StartService();

            Console.WriteLine();
            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }
		
        private	static String ToPath(String	arg)
        {
            return ".\\private$\\" + arg;
        }

    }
}
