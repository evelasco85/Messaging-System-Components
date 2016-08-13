using System;
using System.Collections.Generic;
using System.Text;

namespace LoanBroker.Models.LoanBroker
{
    public struct LoanQuoteRequest
    {
        public int SSN;
        public double LoanAmount;
        public int LoanTerm;
    }
}
