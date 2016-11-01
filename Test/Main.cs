/* Loan Broker Example with MSMQ
 * from Enterprise Integration Patterns (Addison-Wesley, ISBN 0321200683)
 * 
 * Copyright (c) 2003 Gregor Hohpe 
 *
 * This code is supplied as is. No warranties. 
 */

using MessageGateway;
using Messaging.Orchestration.Shared.Models;
using Messaging.Orchestration.Shared.Services;
using MsmqGateway;
using System;
using System.Messaging;

namespace Test
{
    class Run
    {

        [STAThread]
        static void Main(string[] args)
        {
            IClientService client = MQOrchestration.GetInstance().CreateClient(
                args[0],
                ToPath(args[1]),
                ToPath(args[2])
                );

            //IClientService client = MQOrchestration.GetInstance().CreateClient(
            //   @"92022db8-750a-4481-afc7-dc2dcfb8fc20\1",
            //   ToPath("ServerRequestQueue"),
            //   ToPath("ServerReplyQueue")
            //   );

            int numMessages = 0;
            TestLoanBroker test = null;
            string requestQueue = string.Empty;
            string replyQueue = string.Empty;
            string mode = string.Empty;

            client.Register(registration =>
            {
                //Server parameter requests
                registration.RegisterRequiredServerParameters("mode", (value) => mode = (string)value);
                registration.RegisterRequiredServerParameters("requestQueue", (value) => requestQueue = (string)value);
                registration.RegisterRequiredServerParameters("replyQueue", (value) => replyQueue = (string)value);
                registration.RegisterRequiredServerParameters("numMessages", (value) => numMessages = System.Convert.ToInt32(value));
            },
                errorMessage =>
                {
                    //Invalid registration
                },
                () =>
                {
                    //Client parameter setup completed
                    Console.WriteLine("Configurations ok!");
                    test = new TestLoanBroker(ToPath(requestQueue), ToPath(replyQueue), numMessages);
                },
                () =>
                {
                    //Stand-by
                    Console.WriteLine("Test Loan Broker Parameters Prepared!");
                },
                () =>
                {
                    //Start
                    test.Process();
                },
                () =>
                {
                    //Stop
                    test.StopProcessing();
                });

            client.Process();
            Console.ReadLine();
        }


        public static void CreditBureauTest(String[] args)
        {
            if(args.Length != 3) 
            {
                Console.WriteLine("Usage: " + Environment.GetCommandLineArgs()[0] +" <req_queue> <reply_queue> <num_requests>");
                return;
            } 
            
            int numMessages = System.Convert.ToInt32(args[2]);

            TestCreditBureau test = new TestCreditBureau(ToPath(args[0]), ToPath(args[1]), numMessages);
            
            test.Process();
        }

        public static void BankTest(String[] args)
        {
            if(args.Length != 3) 
            {
                Console.WriteLine("Usage: " + Environment.GetCommandLineArgs()[0] +" <req_queue> <reply_queue> <num_requests>");
                return;
            } 
            
            int numMessages = System.Convert.ToInt32(args[2]);

            TestBank test = new TestBank(ToPath(args[0]), ToPath(args[1]), numMessages);
            
            test.Process();
        }
        
        public static void LoanBrokerTest(String[] args)
        {
            if (args.Length != 3) 
            {
                Console.WriteLine("Usage: " + Environment.GetCommandLineArgs()[0] +" <req_queue> <reply_queue> <num_requests>");
                return;
            } 
            
            int numMessages = System.Convert.ToInt32(args[2]);

            TestLoanBroker test = new TestLoanBroker(ToPath(args[0]), ToPath(args[1]), numMessages);
            
            test.Process();
        }


        private static String ToPath(String arg)
        {
            return ".\\private$\\" + arg;
        }

    }

}
