/* Loan Broker Example with MSMQ
 * from Enterprise Integration Patterns (Addison-Wesley, ISBN 0321200683)
 * 
 * Copyright (c) 2003 Gregor Hohpe 
 *
 * This code is supplied as is. No warranties. 
 */

using System;
using Bank.Models;
using MessageGateway;
using Messaging.Base.Construction;
using Messaging.Orchestration.Shared.Services;
using MsmqGateway;

namespace Bank
{
    public class Run
    {
        public static void Main(String[] args)
        {
            String requestQueue = string.Empty;
            String bankName = string.Empty;
            String ratePremium = string.Empty;
            String maxLoanTerm = string.Empty;

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
                    Bank bank = new Bank(bankName, System.Convert.ToDouble(ratePremium), System.Convert.ToInt32(maxLoanTerm));
                    queueService = new MQRequestReplyService_Synchronous(
                        ToPath(requestQueue),
                        new ProcessMessageDelegate(bank.ProcessRequestMessage),
                        null,
                        new GetRequestBodyTypeDelegate(() => { return typeof(BankQuoteRequest); }));

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
                    queueService.Run();
                    Console.WriteLine("Starting Application!");
                },
                () =>
                {
                    //Stop
                    queueService.StopRunning();
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
