using System;
using System.Messaging;
using Bank;
using CommonObjects;
using MessageGateway;
using Messaging.Base;
using LoanBroker.Bank;
using LoanBroker.LoanBroker;
using Messaging.Base.Construction;
using Messaging.Base.System_Management.SmartProxy;
using MsmqGateway;

namespace LoanBroker {

	internal class Run{

	    public static void Main(String[] args)
	    {
	        String requestQueueName = string.Empty;
	        String creditRequestQueueName = string.Empty;
	        String creditReplyQueueName = string.Empty;
	        String bankReplyQueueName = string.Empty;

	        string proxyRequestQueue = string.Empty;
	        string proxyReplyQueue = string.Empty;
	        string controlBusQueue = string.Empty;

	        ClientInstance<MessageQueue, Message> instance = new ClientInstance<MessageQueue, Message>();

	        IMessageFormatter loanRequestFormatter = new XmlMessageFormatter(new Type[] {typeof(LoanQuoteRequest)});
            IMessageFormatter loanReplyFormatter = new XmlMessageFormatter(new Type[] { typeof(LoanQuoteReply) });
            IMessageFormatter creditBureauReplyFormatter = new XmlMessageFormatter(new Type[] { typeof(CreditBureauReply) });
            IMessageFormatter bankQuoteReplyFormatter = new XmlMessageFormatter(new Type[] { typeof(BankQuoteReply) });

	        
            
	        MQOrchestration.GetInstance().CreateClient(
	            args[0],
	            "MSMQ",
	            ToPath(args[1]),
	            ToPath(args[2])
	            )
	            .Register(registration =>
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
	                    //

	                    /*Proxy Setup*/
	                    IMessageReceiver<MessageQueue, Message> proxyReplyReceiver = new MessageReceiverGateway(
	                        ToPath(proxyReplyQueue),
	                        loanReplyFormatter
	                        );
	                    IMessageSender<Message> proxyRequestSender = new MessageSenderGateway(ToPath(proxyRequestQueue));
	                    IMessageReceiver<Message> loanRequestReceiver = new MessageReceiverGateway(
	                        ToPath(requestQueueName), loanRequestFormatter);
	                    IMessageSender<Message> controlBus = new MessageSenderGateway(ToPath(controlBusQueue));

	                    instance.SetupLoanBrokerProxy(
	                        controlBus,
	                        new LoanBrokerProxyRequestConsumer(
	                            loanRequestReceiver,
	                            proxyRequestSender,
	                            proxyReplyReceiver.AsReturnAddress(),
	                            LoanBrokerProxy<MessageQueue, Message>.SQueueStats
	                            ),
	                        new LoanBrokerProxyReplyConsumer(
	                            proxyReplyReceiver,
	                            LoanBrokerProxy<MessageQueue, Message>.SQueueStats,
	                            LoanBrokerProxy<MessageQueue, Message>.S_PerformanceStats,
	                            controlBus
	                            ));
	                    /*************/

	                    /*Bank Gateway Setup*/
	                    instance.SetupBankGateway(
	                        new MessageReceiverGateway(
	                            ToPath(bankReplyQueueName),
	                            bankQuoteReplyFormatter
	                            ),
	                        new ConnectionsManager<Message>(),
	                        ((aggregationCorrelationID, request) =>
	                        {
	                            return new Message(request)
	                            {
	                                AppSpecific = aggregationCorrelationID
	                            };
	                        }),
	                        (message =>
	                        {
	                            message.Formatter = bankQuoteReplyFormatter;

	                            return new Tuple<int, bool, BankQuoteReply>(
	                                message.AppSpecific,
	                                message.Body is BankQuoteReply,
	                                (BankQuoteReply) message.Body
	                                );
	                        }));
	                    /********************/

	                    /*Credit Bureau Setup*/
	                    instance.SetupCreditBureauInterface(
	                        new MessageSenderGateway(ToPath(creditRequestQueueName)),
	                        new MessageReceiverGateway(ToPath(creditReplyQueueName), creditBureauReplyFormatter),
	                        ((appSpecific, request) =>
	                        {
	                            return new Message(request)
	                            {
	                                AppSpecific = appSpecific
	                            };
	                        }),
	                        (message =>
	                        {
	                            message.Formatter = creditBureauReplyFormatter;

	                            return new Tuple<int, bool, CreditBureauReply>(
	                                message.AppSpecific,
	                                message.Body is CreditBureauReply,
	                                (CreditBureauReply) message.Body
	                                );
	                        }));
	                    /*********************/

                        instance.SetupProcessManager();
	                    instance.SetupQueueService(
	                        new MQRequestReplyService_Asynchronous(
	                            ToPath(proxyRequestQueue),
	                            new AsyncProcessMessageDelegate(instance.ProcessManager.ProcessRequestMessage),
	                            null,
	                            new GetRequestBodyTypeDelegate(instance.ProcessManager.GetRequestBodyType)
	                            )
	                        );
	                    instance.HookMessageIdExtractor(message => { return message.Id; });
	                    instance.StartProcessingManagerListening();

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
                        instance.Start();
	                    Console.WriteLine("Starting Application!");
	                },
	                () =>
	                {
	                    //Stop
	                    instance.Stop();
                        Console.WriteLine("Stopping Application!");
	                })
	            .StartService();


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
