/* Loan Broker Example with MSMQ
 * from Enterprise Integration Patterns (Addison-Wesley, ISBN 0321200683)
 * 
 * Copyright (c) 2003 Gregor Hohpe 
 *
 * This code is supplied as is. No warranties. 
 */

using System;
using System.Messaging;
using Messaging.Base;
using Messaging.Base.Routing;
using System.Linq;
using System.Collections.Generic;
using MessageGateway;

namespace LoanBroker.Bank
{
    // Modified connection manager that sends requests to Bank 5 only if there are not other takers
    internal class ConnectionsManager
    {
        static protected IRecipientList<Connection<Message>, Message> bankRecipientList = new RecipientList<Connection<Message>, Message>(
            (recipient => recipient.Queue),
            new Bank1<Message>(new MessageSenderGateway(ToPath("bank1Queue"))),
            new Bank2<Message>(new MessageSenderGateway(ToPath("bank2Queue"))),
            new Bank3<Message>(new MessageSenderGateway(ToPath("bank3Queue"))),
            new Bank4<Message>(new MessageSenderGateway(ToPath("bank4Queue")))
            );
        static protected Connection<Message> catchAll = new Bank5<Message>(new MessageSenderGateway(ToPath("bank5Queue")));

        public IMessageSender<Message>[] GetEligibleBankQueues(int CreditScore, int HistoryLength, int LoanAmount)
        {
            IList<IMessageSender<Message>> recipientList = bankRecipientList
                .GetRecipients(recipient => recipient.CanHandleLoanRequest(CreditScore, HistoryLength, LoanAmount));

            if (recipientList.Count == 0)
                recipientList.Add(catchAll.Queue);

            return recipientList.ToArray();
        }

        static String ToPath(String arg)
        {
            return ".\\private$\\" + arg;
        }
    }
}
