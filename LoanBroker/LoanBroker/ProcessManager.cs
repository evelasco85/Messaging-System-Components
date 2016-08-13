using LoanBroker.Bank;
using LoanBroker.Models.LoanBroker;
using MessageGateway;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Messaging;
using System.Text;

namespace LoanBroker.LoanBroker
{
    internal class ProcessManager : MQRequestReplyService_Asynchronous
    {
        protected ICreditBureauGateway creditBureauInterface;
        protected BankGateway bankInterface;
        protected IDictionary activeProcesses = (IDictionary)(new Hashtable());

        public ProcessManager(String requestQueueName,
            String creditRequestQueueName, String creditReplyQueueName,
            String bankReplyQueueName, ConnectionsManager connectionManager)
            : base(requestQueueName)
        {
            creditBureauInterface = (ICreditBureauGateway)
                (new CreditBureauGatewayImp(creditRequestQueueName, creditReplyQueueName));
            creditBureauInterface.Listen();

            bankInterface = new BankGateway(bankReplyQueueName, connectionManager);
            bankInterface.Listen();
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
                new Process(this, processID, creditBureauInterface,
                bankInterface, quoteRequest, message);
            activeProcesses.Add(processID, newProcess);
        }

        public void OnProcessComplete(String processID)
        {
            activeProcesses.Remove(processID);
        }
    }
}
