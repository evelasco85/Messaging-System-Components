using System;
using CommonObjects;
using MessageGateway;
using Messaging.Base.Construction;
using Messaging.Orchestration.Shared.Services;
using MsmqGateway;

namespace CreditBureau
{

    public class Run
    {
        public static void Main(String[] args)
        {
            String requestQueueName = string.Empty;

            IRequestReply_Synchronous queueService = null;
            IClientService client = MQOrchestration.GetInstance().CreateClient(
                args[0],
                "MSMQ",
                ToPath(args[1]),
                ToPath(args[2])
                );

            client.Register(registration =>
            {
                //Server parameter requests
                registration.RegisterRequiredServerParameters("requestQueueName", (value) => requestQueueName = (string)value);
            },
                errorMessage =>
                {
                    //Invalid registration
                },
                () =>
                {
                    //Client parameter setup completed
                    CreditBureau proc = new CreditBureau();
                    queueService = new MQRequestReplyService_Synchronous(
                        ToPath(requestQueueName),
                        new SyncProcessMessageDelegate(proc.ProcessRequestMessage),
                        null,
                        new GetRequestBodyTypeDelegate(() => { return typeof(CreditBureauRequest); }));


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
                    if (queueService != null)
                    {
                        queueService.Run();
                        Console.WriteLine("Starting Application!");
                    }
                },
                () =>
                {
                    //Stop
                    if (queueService != null)
                    {
                        queueService.StopRunning();
                        Console.WriteLine("Stopping Application!");
                    }
                });

            client.Process();

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
