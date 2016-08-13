using CreditBureau;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoanBroker.CreditBureau
{
    public delegate void OnCreditReplyEvent(CreditBureauReply creditReply, Object ACT);

    internal struct CreditRequestProcess
    {
        public int CorrelationID;
        public Object ACT;
        public OnCreditReplyEvent callback;
    }
}
