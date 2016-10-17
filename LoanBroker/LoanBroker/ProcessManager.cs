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
        public ICreditBureauGateway CreditBureauInterface { get; private set; }
        public BankGateway BankInterface { get; private set; }

        IProcessManager<string, Process, ProcessManager> _manager;

        public ProcessManager(
            String requestQueueName,
            String creditRequestQueueName, String creditReplyQueueName,
            String bankReplyQueueName, ConnectionsManager connectionManager)
            :this(requestQueueName,
                 (new CreditBureauGatewayImp(creditRequestQueueName, creditReplyQueueName)),
                 bankReplyQueueName,
                 connectionManager)
        {}

        public ProcessManager(
            String requestQueueName,
            ICreditBureauGateway creditBureau,
            String bankReplyQueueName, ConnectionsManager connectionManager)
            : base(requestQueueName)
        {
            CreditBureauInterface = creditBureau;
            CreditBureauInterface.Listen();

            BankInterface = new BankGateway(bankReplyQueueName, connectionManager);
            BankInterface.Listen();

            _manager = new ProcessManager<string, Process, ProcessManager>(
                this,
                ChildProcessNotification
                );
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
            Process newProcess = new Process(processID, quoteRequest, message);

            _manager.AddProcess(newProcess);

            newProcess.StartProcess();
        }

        void ChildProcessNotification(IProcess<string, Process, ProcessManager> process)
        {
            _manager.RemoveProcess(process);
            Console.WriteLine("Current outstanding aggregate count: {0}", BankInterface.GetOutstandingAggregateCount());
        }
    }
}
