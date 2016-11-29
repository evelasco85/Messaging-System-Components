using System;
using System.Collections.Generic;
using System.Text;

namespace LoanBroker.Bank
{
    internal class Bank1 : Connection
    {
        protected String bankname = "Exclusive Country Club Bankers";

        public Bank1() : base("bank1Queue") { }
        public override bool CanHandleLoanRequest(int CreditScore, int HistoryLength, int LoanAmount)
        {
            return LoanAmount >= 75000 && CreditScore >= 600 && HistoryLength >= 8;
        }
    }

    internal class Bank2 : Connection
    {
        protected String bankname = "Acme Deluxe Bankers";

        public Bank2() : base("bank2Queue") { }
        public override bool CanHandleLoanRequest(int CreditScore, int HistoryLength, int LoanAmount)
        {
            return LoanAmount >= 75000 && CreditScore >= 600 && HistoryLength >= 8;
        }
    }

    internal class Bank3 : Connection
    {
        protected String bankname = "General Retail Bankers";

        public Bank3() : base("bank3Queue") { }
        public override bool CanHandleLoanRequest(int CreditScore, int HistoryLength, int LoanAmount)
        {
            return LoanAmount >= 10000 && LoanAmount < 75000 && CreditScore >= 400 && HistoryLength >= 3;
        }
    }

    internal class Bank4 : Connection
    {
        protected String bankname = "Neighborhood Bankers";

        public Bank4() : base("bank4Queue") { }
        public override bool CanHandleLoanRequest(int CreditScore, int HistoryLength, int LoanAmount)
        {
            return LoanAmount >= 10000 && LoanAmount < 75000 && CreditScore >= 400 && HistoryLength >= 3;
        }
    }

    internal class Bank5 : Connection
    {
        protected String bankname = "Mom and Pop Generic Loan Sharks and Pawn Shop";

        public Bank5() : base("bank5Queue") { }
        public override bool CanHandleLoanRequest(int CreditScore, int HistoryLength, int LoanAmount)
        {
            return true;
        }
    }
}
