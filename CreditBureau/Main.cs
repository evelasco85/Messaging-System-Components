/* Loan Broker Example with MSMQ
 * from Enterprise Integration Patterns (Addison-Wesley, ISBN 0321200683)
 * 
 * Copyright (c) 2003 Gregor Hohpe 
 *
 * This code is supplied as is. No warranties. 
 */

using System;
using System.Messaging;
using MessageGateway;
using Messaging.Orchestration.Shared.Services;
using MsmqGateway;

namespace CreditBureau
{

    public class Run
    {
        private static CreditBureau _proc;
        public static void Main(String[] args)
        {
            String requestQueueName = string.Empty;
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
                    _proc = new CreditBureau(ToPath(requestQueueName));
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
                    _proc.Run();
                    Console.WriteLine("Starting Application!");
                },
                () =>
                {
                    //Stop
                    _proc.StopRunning();
                    Console.WriteLine("Stopping Application!");
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
