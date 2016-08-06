/* Loan Broker Example with MSMQ
 * from Enterprise Integration Patterns (Addison-Wesley, ISBN 0321200683)
 * 
 * Copyright (c) 2003 Gregor Hohpe 
 *
 * This code is supplied as is. No warranties. 
 */

using System;
using Gateway;

namespace Bank
{
    public class Run
    {
        public static void Main(String[] args)
        {
            if(args.Length != 4) 
            {
                Console.WriteLine("Usage: "	+ Environment.GetCommandLineArgs()[0] +" <req_queue> <name> <premium> <maxloanterm>");
                return;
            } 
            Bank bank = new Bank(ToPath(args[0]), args[1], System.Convert.ToDouble(args[2]), System.Convert.ToInt32(args[3]));
            bank.Run();
            
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
