using MsmqGateway;
using System;
using System.Messaging;
using CommonObjects;
using Messaging.Orchestration.Shared.Services;

namespace Test
{
    class Run
    {

        [STAThread]
        static void Main(string[] args)
        {
            ClientInstance<Message> instance = new ClientInstance<Message>();
            IClientService service = MQClient.GetClientService(instance, args);

            service.StartService();

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
            //if (args.Length != 3) 
            //{
            //    Console.WriteLine("Usage: " + Environment.GetCommandLineArgs()[0] +" <req_queue> <reply_queue> <num_requests>");
            //    return;
            //} 
            
            //int numMessages = System.Convert.ToInt32(args[2]);

            //TestLoanBroker test = new TestLoanBroker(ToPath(args[0]), ToPath(args[1]), numMessages);
            
            //test.Process();
        }


        private static String ToPath(String arg)
        {
            return ".\\private$\\" + arg;
        }

    }

}
