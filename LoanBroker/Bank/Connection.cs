using Messaging.Base;

namespace LoanBroker.Bank
{
    internal abstract class Connection<TMessage>
    {
        protected IMessageSender<TMessage> queue;

        public IMessageSender<TMessage> Queue
        {
            get { return queue; }
        }

        public Connection(IMessageSender<TMessage> queue)
        {
            this.queue = queue;
        }

        public abstract bool CanHandleLoanRequest(int CreditScore, int HistoryLength, int LoanAmount);
        
    }
}
