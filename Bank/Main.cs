/* Loan Broker Example with MSMQ
 * from Enterprise Integration Patterns (Addison-Wesley, ISBN 0321200683)
 * 
 * Copyright (c) 2003 Gregor Hohpe 
 *
 * This code is supplied as is. No warranties. 
 */

using System;
using Gateway;
using Messaging.Orchestration.Shared.Services;
using MsmqGateway;

namespace Bank
{
    public class Run
    {
        private static Bank _bank;
        public static void Main(String[] args)
        {
            String requestQueue = string.Empty;
            String bankName = string.Empty;
            String ratePremium = string.Empty;
            String maxLoanTerm = string.Empty;

            IClientService client = MQOrchestration.GetInstance().CreateClient(
                args[0],
                "MSMQ",
                ToPath(args[1]),
                ToPath(args[2])
                );

            client.Register(registration =>
            {
                //Server parameter requests
                registration.RegisterRequiredServerParameters("requestQueue", (value) => requestQueue = (string)value);
                registration.RegisterRequiredServerParameters("bankName", (value) => bankName = (string)value);
                registration.RegisterRequiredServerParameters("ratePremium", (value) => ratePremium = (string)value);
                registration.RegisterRequiredServerParameters("maxLoanTerm", (value) => maxLoanTerm = (string)value);
            },
                errorMessage =>
                {
                    //Invalid registration
                },
                () =>
                {
                    //Client parameter setup completed
                    _bank = new Bank(ToPath(requestQueue), bankName, System.Convert.ToDouble(ratePremium), System.Convert.ToInt32(maxLoanTerm));

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
                    _bank.QueueService.Run();
                    Console.WriteLine("Starting Application!");
                },
                () =>
                {
                    //Stop
                    _bank.QueueService.StopRunning();
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
