using Messaging.Base;

namespace LoanBroker.Bank
{
    internal abstract class Connection<TMessage>
    {
        protected IRawMessageSender<TMessage> queue;

        public IRawMessageSender<TMessage> Queue
        {
            get { return queue; }
        }

        public Connection(IRawMessageSender<TMessage> queue)
        {
            this.queue = queue;
        }

        public abstract bool CanHandleLoanRequest(int CreditScore, int HistoryLength, int LoanAmount);
        
    }
}
