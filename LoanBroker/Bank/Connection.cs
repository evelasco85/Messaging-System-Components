using MessageGateway;
using System;
using System.Collections.Generic;
using System.Messaging;
using System.Text;
using Messaging.Base;

namespace LoanBroker.Bank
{
    internal abstract class Connection
    {
        protected IMessageSender<MessageQueue, Message> queue;
        
        public IMessageSender<MessageQueue, Message> Queue
        {
            get { return queue; }
        }

        public Connection(String queueName)
        {
            this.queue = new MessageSenderGateway(ToPath(queueName));
        }


        public abstract bool CanHandleLoanRequest(int CreditScore, int HistoryLength, int LoanAmount);

        private static String ToPath(String arg)
        {
            return ".\\private$\\" + arg;
        }
    }
}
