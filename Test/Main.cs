/* Loan Broker Example with MSMQ
 * from Enterprise Integration Patterns (Addison-Wesley, ISBN 0321200683)
 * 
 * Copyright (c) 2003 Gregor Hohpe 
 *
 * This code is supplied as is. No warranties. 
 */

using System;

namespace Test
{
    class Run
    {

        [STAThread]
        static void Main(string[] args)
        {
            String arg = args[0];
            String[] newArgs =  new String[args.Length -1];
            for(int i = 1; i < args.Length; i++)
                newArgs[i-1] = args[i];
			
            if(arg.Equals("credit"))
                CreditBureauTest(newArgs);
            else if (arg.Equals("bank"))
                BankTest(newArgs);
            else if (arg.Equals("loanbroker"))
                LoanBrokerTest(newArgs);
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
