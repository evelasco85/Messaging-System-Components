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
using Gateway.Mock;
using CreditBureau;
using Messaging.Base;

namespace LoanBroker {

	internal class Run{
		private static MessageDelegate<Message> PrintMessageDelegate = new MessageDelegate<Message>(PrintMessage);
		
		public static void Main(String[] args){
            MQRequestReplyService_Asynchronous broker;

            if (args.Length >= 4) 
            {
                String requestQueueName = ToPath(args[0]); 
                String creditRequestQueueName = ToPath(args[1]);
                String creditReplyQueueName = ToPath(args[2]);
                String bankReplyQueueName = ToPath(args[3]);
                broker = new LoanBrokerPM(requestQueueName, creditRequestQueueName, creditReplyQueueName, bankReplyQueueName, new BankConnectionManager());
            }
            else if (args.Length == 2) 
            {
                broker = new LoanBrokerPM(ToPath(args[0]), new MockCreditBureauGatewayImp(), ToPath(args[1]), new BankConnectionManager());
            }
            else 
            {
                Console.WriteLine("Usage: <>");
                return;
            }

            broker.Run();
            
            Console.WriteLine();
            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
		}
				
		private static String ToPath(String arg){
			return ".\\private$\\" + arg;
		}
		
		public static void PrintMessage(Message m)
		{
			m.Formatter =  new System.Messaging.XmlMessageFormatter(new String[] {"System.String,mscorlib"});	
			string body = (string)m.Body;
			Console.WriteLine("Received Message: " + body);
			return; 
		}	
	}
}
