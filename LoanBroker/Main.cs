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
using Messaging.Base;
using LoanBroker.Bank;
using LoanBroker.LoanBroker;
using MsmqGateway;
using LoanBroker.Models.LoanBroker;

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

                string proxyRequestQueue = ".\\private$\\broker_loanrequestqueue";
                string proxyReplyQueue = ".\\private$\\broker_loanreplyqueue";

                IMessageReceiver<MessageQueue, Message> proxyReplyReceiver = new MessageReceiverGateway(
                    proxyReplyQueue,
                    GetLoanReplyFormatter()
                    );

                _loanBrokerProxy = new LoanBrokerProxy(
                    new MessageReceiverGateway(requestQueueName, GetLoanRequestFormatter()),
                    new MessageSenderGateway(proxyRequestQueue),
                    new MQReturnAddress(proxyReplyReceiver),
                    proxyReplyReceiver,
                    new MessageSenderGateway(".\\private$\\controlbus"),
                    3);

                _loanBrokerProxy.Process();

                broker = new ProcessManager(proxyRequestQueue, creditRequestQueueName, creditReplyQueueName, bankReplyQueueName, new ConnectionsManager());
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

        static IMessageFormatter GetLoanRequestFormatter()
        {
            return new XmlMessageFormatter(new Type[] { typeof(LoanQuoteRequest) });
        }
        static IMessageFormatter GetLoanReplyFormatter()
        {
            return new XmlMessageFormatter(new Type[] { typeof(LoanQuoteReply) });
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
