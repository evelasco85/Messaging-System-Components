/* Loan Broker Example with MSMQ
 * from Enterprise Integration Patterns (Addison-Wesley, ISBN 0321200683)
 * 
 * Copyright (c) 2003 Gregor Hohpe 
 *
 * This code is supplied as is. No warranties. 
 */

using System;
using System.Messaging;
using System.Threading;
using MessageGateway;
using Messaging.Base;
using CreditBureau.Models;
using Messaging.Base.Construction;

namespace CreditBureau
{
    internal class CreditBureau
    {
        private Random random = new Random();

        private int getCreditScore(int ssn) 
        {
            return (int)(random.Next(600) + 300);
        }

        private int getCreditHistoryLength(int ssn)
        {
            return (int)(random.Next(19) + 1);
        }

        public Object ProcessRequestMessage(Object o)
        {
            CreditBureauRequest requestStruct;

            requestStruct = (CreditBureauRequest)o;

            Console.WriteLine("Received request for SSN " + requestStruct.SSN);
            CreditBureauReply replyStruct = new CreditBureauReply();
            replyStruct.SSN = requestStruct.SSN;
            replyStruct.CreditScore = getCreditScore(replyStruct.SSN);
            replyStruct.HistoryLength = getCreditHistoryLength(replyStruct.SSN);

            Thread.Sleep(replyStruct.CreditScore);

            Console.WriteLine("  Score {0} History {1} months", replyStruct.CreditScore, replyStruct.HistoryLength);
            return replyStruct;
        }
    }
}
