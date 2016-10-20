/* Loan Broker Example with MSMQ
 * from Enterprise Integration Patterns (Addison-Wesley, ISBN 0321200683)
 * 
 * Copyright (c) 2003 Gregor Hohpe 
 *
 * This code is supplied as is. No warranties. 
 */

using System;
using System.Messaging;
using CommonObjects;
using LoanBroker.CreditBureau;

namespace LoanBroker
{

    public class MockCreditBureauGatewayImp : ICreditBureauGateway
    {   

        private Random random = new Random();

        public MockCreditBureauGatewayImp() 
        { }

        public void GetCreditScore(CreditBureauRequest quoteRequest, OnCreditReplyEvent OnCreditResponse)
        {
            CreditBureauReply  reply = new CreditBureauReply();
            reply.CreditScore =  (int)(random.Next(600) + 300);
            reply.HistoryLength = (int)(random.Next(19) + 1);
            reply.SSN = quoteRequest.SSN;
            OnCreditResponse(reply);
        }

        public void Listen()
        { }
    }

}
