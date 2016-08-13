using System;
using System.Collections.Generic;
using System.Text;

namespace LoanBroker.Models.LoanBroker
{
    public struct LoanQuoteReply
    {
        public int SSN;
        public double LoanAmount;
        public double InterestRate;
        public string QuoteID;
    }
}
