using LoanBroker.Bank;
using LoanBroker.Models.LoanBroker;
using MessageGateway;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Messaging;
using System.Text;
using Messaging.Base.Routing;

namespace LoanBroker.LoanBroker
{
    internal class ProcessManager : MQRequestReplyService_Asynchronous
    {
        protected ICreditBureauGateway creditBureauInterface;
        protected BankGateway bankInterface;

        IProcessManager<string, Process> _manager = new ProcessManager<string, Process>();

        public ProcessManager(System.String requestQueueName,
            String creditRequestQueueName, String creditReplyQueueName,
            String bankReplyQueueName, ConnectionsManager connectionManager)
            : base(requestQueueName)
        {
            creditBureauInterface = (ICreditBureauGateway)
                (new CreditBureauGatewayImp(creditRequestQueueName, creditReplyQueueName));
            creditBureauInterface.Listen();

            bankInterface = new BankGateway(bankReplyQueueName, connectionManager);
            bankInterface.Listen();

            _manager.ManagerNotifier = new NotifyManagerDelegate<string, Process>(ProcessNotification);
        }

        public ProcessManager(String requestQueueName,
            ICreditBureauGateway creditBureau,
            String bankReplyQueueName, ConnectionsManager connectionManager)
            : base(requestQueueName)
        {
            creditBureauInterface = creditBureau;
            creditBureauInterface.Listen();

            bankInterface = new BankGateway(bankReplyQueueName, connectionManager);
            bankInterface.Listen();

            _manager.ManagerNotifier = new NotifyManagerDelegate<string, Process>(ProcessNotification);
        }

        public override Type GetRequestBodyType()
        {
            return typeof(LoanQuoteRequest);
        }

        public override void ProcessRequestMessage(Object o, Message message)
        {
            LoanQuoteRequest quoteRequest;
            quoteRequest = (LoanQuoteRequest)o;

            String processID = message.Id;
            Process newProcess =
                new Process(
                    new NotifyManagerDelegate<string, Process>(ProcessNotification),
                    this,
                    processID, creditBureauInterface,
                bankInterface, quoteRequest, message);

            _manager.AddProcess(newProcess);
        }

        void ProcessNotification(IProcess<string, Process> process)
        {
            _manager.RemoveProcess(process);
            Console.WriteLine("Current outstanding aggregate count: {0}", bankInterface.GetOutstandingAggregateCount());
        }
    }
}
