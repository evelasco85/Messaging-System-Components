using System;
using Messaging.Base;

namespace LoanBroker.Bank
{
    internal class Bank1<TMessage> : Connection<TMessage>
    {
        protected String bankname = "Exclusive Country Club Bankers";

        public Bank1(IMessageSender<TMessage> queue) : base(queue) { }
        public override bool CanHandleLoanRequest(int CreditScore, int HistoryLength, int LoanAmount)
        {
            return LoanAmount >= 75000 && CreditScore >= 600 && HistoryLength >= 8;
        }
    }

    internal class Bank2<TMessage> : Connection<TMessage>
    {
        protected String bankname = "Acme Deluxe Bankers";

        public Bank2(IMessageSender<TMessage> queue) : base(queue) { }
        public override bool CanHandleLoanRequest(int CreditScore, int HistoryLength, int LoanAmount)
        {
            return LoanAmount >= 75000 && CreditScore >= 600 && HistoryLength >= 8;
        }
    }

    internal class Bank3<TMessage> : Connection<TMessage>
    {
        protected String bankname = "General Retail Bankers";

        public Bank3(IMessageSender<TMessage> queue) : base(queue) { }
        public override bool CanHandleLoanRequest(int CreditScore, int HistoryLength, int LoanAmount)
        {
            return LoanAmount >= 10000 && LoanAmount < 75000 && CreditScore >= 400 && HistoryLength >= 3;
        }
    }

    internal class Bank4<TMessage> : Connection<TMessage>
    {
        protected String bankname = "Neighborhood Bankers";

        public Bank4(IMessageSender<TMessage> queue) : base(queue) { }
        public override bool CanHandleLoanRequest(int CreditScore, int HistoryLength, int LoanAmount)
        {
            return LoanAmount >= 10000 && LoanAmount < 75000 && CreditScore >= 400 && HistoryLength >= 3;
        }
    }

    internal class Bank5<TMessage> : Connection<TMessage>
    {
        protected String bankname = "Mom and Pop Generic Loan Sharks and Pawn Shop";

        public Bank5(IMessageSender<TMessage> queue) : base(queue) { }
        public override bool CanHandleLoanRequest(int CreditScore, int HistoryLength, int LoanAmount)
        {
            return true;
        }
    }
}
