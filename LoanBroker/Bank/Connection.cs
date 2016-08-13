using MessageGateway;
using System;
using System.Collections.Generic;
using System.Messaging;
using System.Text;

namespace LoanBroker.Bank
{
    internal abstract class Connection
    {
        protected MessageSenderGateway queue;
        protected String bankName = "";
        public MessageSenderGateway Queue
        {
            get { return queue; }
        }
        public String BankName
        {
            get { return bankName; }
        }
        public Connection(MessageQueue queue) { this.queue = new MessageSenderGateway(queue); }
        public Connection(String queueName) { this.queue = new MessageSenderGateway(queueName); }

        public abstract bool CanHandleLoanRequest(int CreditScore, int HistoryLength, int LoanAmount);
    }
}
