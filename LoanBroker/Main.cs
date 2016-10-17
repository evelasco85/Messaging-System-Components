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
using LoanBroker.Bank;
using LoanBroker.LoanBroker;
using MsmqGateway;

namespace LoanBroker {

	internal class Run{
        private static LoanBrokerProxy _loanBrokerProxy;

		public static void Main(String[] args){
            MQRequestReplyService_Asynchronous broker;

            if (args.Length >= 4) 
            {
                String requestQueueName = ToPath(args[0]); 
                String creditRequestQueueName = ToPath(args[1]);
                String creditReplyQueueName = ToPath(args[2]);
                String bankReplyQueueName = ToPath(args[3]);

                broker = new ProcessManager(requestQueueName, creditRequestQueueName, creditReplyQueueName, bankReplyQueueName, new ConnectionsManager());

                //string proxyRequestQueue = ".\\private$\\broker_loanrequestqueue";
                //string proxyReplyQueue = ".\\private$\\broker_loanreplyqueue";

                //_loanBrokerProxy = new LoanBrokerProxy(
                //    new MessageReceiverGateway(requestQueueName, GetFormatter()),
                //    new MessageSenderGateway(proxyRequestQueue),
                //    new MQReturnAddress(new MessageReceiverGateway(proxyReplyQueue)),
                //    new MessageSenderGateway(bankReplyQueueName),
                //    new MessageReceiverGateway(proxyReplyQueue),
                //    new MessageSenderGateway(".\\private$\\controlbus"),
                //    5);
                //broker = new ProcessManager(proxyRequestQueue, creditRequestQueueName, creditReplyQueueName, proxyReplyQueue, new ConnectionsManager());

                //_loanBrokerProxy.Process();

                
                
            }
            else if (args.Length == 2) 
            {
                broker = new ProcessManager(ToPath(args[0]), new MockCreditBureauGatewayImp(), ToPath(args[1]), new ConnectionsManager());
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

        static IMessageFormatter GetFormatter()
        {
            return new XmlMessageFormatter(new Type[] { typeof(CreditBureauReply) });
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
