/* Loan Broker Example with MSMQ
 * from Enterprise Integration Patterns (Addison-Wesley, ISBN 0321200683)
 * 
 * Copyright (c) 2003 Gregor Hohpe 
 *
 * This code is supplied as is. No warranties. 
 */

using System;
using System.Messaging;
using CommonObjects;
using MessageGateway;
using Messaging.Base;
using LoanBroker.Bank;
using LoanBroker.LoanBroker;
using Messaging.Base.Construction;
using Messaging.Orchestration.Shared.Services;
using MsmqGateway;

namespace LoanBroker {

	internal class Run{
        private static LoanBrokerProxy _loanBrokerProxy;

	    public static void Main(String[] args)
	    {
	        String requestQueueName = string.Empty;
	        String creditRequestQueueName = string.Empty;
	        String creditReplyQueueName = string.Empty;
	        String bankReplyQueueName = string.Empty;

	        string proxyRequestQueue = string.Empty;
	        string proxyReplyQueue = string.Empty;
	        string controlBusQueue = string.Empty;

            IRequestReply_Asynchronous<Message> queueService = null;
	        IClientService client = MQOrchestration.GetInstance().CreateClient(
	            args[0],
                "MSMQ",
	            ToPath(args[1]),
	            ToPath(args[2])
	            );

	        client.Register(registration =>
	        {
	            //Server parameter requests
	            registration.RegisterRequiredServerParameters("requestQueueName",
	                (value) => requestQueueName = (string) value);
	            registration.RegisterRequiredServerParameters("creditRequestQueueName",
	                (value) => creditRequestQueueName = (string) value);
	            registration.RegisterRequiredServerParameters("creditReplyQueueName",
	                (value) => creditReplyQueueName = (string) value);
	            registration.RegisterRequiredServerParameters("bankReplyQueueName",
	                (value) => bankReplyQueueName = (string) value);
	            registration.RegisterRequiredServerParameters("controlBusQueue",
	                (value) => controlBusQueue = (string) value);
	            registration.RegisterRequiredServerParameters("proxyRequestQueue",
	                (value) => proxyRequestQueue = (string) value);
	            registration.RegisterRequiredServerParameters("proxyReplyQueue",
	                (value) => proxyReplyQueue = (string) value);

	        },
	            errorMessage =>
	            {
	                //Invalid registration
	            },
	            () =>
	            {
	                //Client parameter setup completed

	                IMessageReceiver<MessageQueue, Message> proxyReplyReceiver = new MessageReceiverGateway(
	                    ToPath(proxyReplyQueue),
	                    GetLoanReplyFormatter()
	                    );
	                _loanBrokerProxy = new LoanBrokerProxy(
	                    new MessageReceiverGateway(ToPath(requestQueueName), GetLoanRequestFormatter()),
	                    new MessageSenderGateway(ToPath(proxyRequestQueue)),
	                    proxyReplyReceiver,
	                    new MessageSenderGateway(ToPath(controlBusQueue)),
	                    3);

                    ProcessManager<Message> processManager = new ProcessManager<Message>(
	                    ToPath(creditRequestQueueName),
	                    ToPath(creditReplyQueueName),
                        ToPath(bankReplyQueueName),
	                    new ConnectionsManager()
                        );

                    queueService = new MQRequestReplyService_Asynchronous(
                        ToPath(proxyRequestQueue),
                        new ProcessMessageDelegate2(processManager.ProcessRequestMessage),
                        null,
                        new GetRequestBodyTypeDelegate(processManager.GetRequestBodyType)
                        ); 
	                
                    processManager.AddSetup(
                        queueService,
                        (message => { return message.Id; })
                        );

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
	                _loanBrokerProxy.Process();
                    queueService.Run();

	                Console.WriteLine("Starting Application!");
	            },
	            () =>
	            {
	                //Stop
	                _loanBrokerProxy.StopProcessing();
                    queueService.StopRunning();

	                Console.WriteLine("Stopping Application!");
	            });

	        client.Process();


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
