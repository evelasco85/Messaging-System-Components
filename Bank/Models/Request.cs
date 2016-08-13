using System;
using System.Collections.Generic;
using System.Text;

namespace Bank.Models
{
    public struct BankQuoteRequest
    {
        public int SSN;
        public int CreditScore;
        public int HistoryLength;
        public int LoanAmount;
        public int LoanTerm;
    }
}
