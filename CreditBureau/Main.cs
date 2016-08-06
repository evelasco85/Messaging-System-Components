/* Loan Broker Example with MSMQ
 * from Enterprise Integration Patterns (Addison-Wesley, ISBN 0321200683)
 * 
 * Copyright (c) 2003 Gregor Hohpe 
 *
 * This code is supplied as is. No warranties. 
 */

using System;

namespace CreditBureau
{

    public class Run
    {
        public static void Main(String[] args)
        {
            if(args.Length != 1) 
            {
                Console.WriteLine("Usage: "	+ Environment.GetCommandLineArgs()[0] +" <in_queue>");
                return;
            } 

            CreditBureau proc = new CreditBureau(ToPath(args[0]));
            proc.Run();
            
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
