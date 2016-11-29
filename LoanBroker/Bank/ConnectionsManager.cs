/* Loan Broker Example with MSMQ
 * from Enterprise Integration Patterns (Addison-Wesley, ISBN 0321200683)
 * 
 * Copyright (c) 2003 Gregor Hohpe 
 *
 * This code is supplied as is. No warranties. 
 */

using System.Messaging;
using Messaging.Base;
using Messaging.Base.Routing;
using System.Linq;
using System.Collections.Generic;

namespace LoanBroker.Bank
{
    // Modified connection manager that sends requests to Bank 5 only if there are not other takers
    internal class ConnectionsManager
    {
        static protected IRecipientList<Connection, MessageQueue, Message> bankRecipientList = new RecipientList<Connection, MessageQueue, Message>(
            (recipient => recipient.Queue),
            new Bank1(), new Bank2(), new Bank3(), new Bank4()
            );
        static protected Connection catchAll = new Bank5();

        public IMessageSender<Message>[] GetEligibleBankQueues(int CreditScore, int HistoryLength, int LoanAmount)
        {
            IList<IMessageSender<Message>> recipientList = bankRecipientList
                .GetRecipients(recipient => recipient.CanHandleLoanRequest(CreditScore, HistoryLength, LoanAmount));

            if (recipientList.Count == 0)
                recipientList.Add(catchAll.Queue);

            return recipientList.ToArray();
        }
    }
}
