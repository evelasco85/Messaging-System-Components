/* Loan Broker Example with MSMQ
 * from Enterprise Integration Patterns (Addison-Wesley, ISBN 0321200683)
 * 
 * Copyright (c) 2003 Gregor Hohpe 
 *
 * This code is supplied as is. No warranties. 
 */

using System;
using System.Messaging;
using System.Collections;
using MessageGateway;
using Messaging.Base;
using LoanBroker.Bank;
using Messaging.Base.Routing;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace LoanBroker.Bank
{
    // Original connection manager that sends all requests to Bank 5
    /*
    internal class BankConnectionManager
    {
        static protected BankConnection[] banks = {new Bank1(), new Bank2(), new Bank3(), new Bank4(), new Bank5() };

        public IMessageSender<MessageQueue, Message> [] GetEligibleBankQueues(int CreditScore, int HistoryLength, int LoanAmount)
        {
            ArrayList lenders = new ArrayList();

            for (int index = 0; index < banks.Length; index++) 
            {
                if (banks[index].CanHandleLoanRequest(CreditScore, HistoryLength, LoanAmount))
                    lenders.Add(banks[index].Queue);
            }
            IMessageSender<MessageQueue, Message> [] lenderArray = (IMessageSender<MessageQueue, Message>  [])Array.CreateInstance(typeof(IMessageSender<MessageQueue, Message> ), lenders.Count);
            lenders.CopyTo(lenderArray);
            return lenderArray;
        }
    }
    */

    // Modified connection manager that sends requetsts to Bank 5 only if there are not other takers
    internal class ConnectionsManager
    {
        static protected IRecipientList<Connection, MessageQueue, Message> bankRecipientList = new RecipientList<Connection, MessageQueue, Message>(
            (recipient => recipient.Queue),
            new Bank1(), new Bank2(), new Bank3(), new Bank4()
            );
        static protected Connection catchAll = new Bank5();

        public IMessageSender<MessageQueue, Message> [] GetEligibleBankQueues(int CreditScore, int HistoryLength, int LoanAmount)
        {
            IList<IMessageSender<MessageQueue, Message>> recipientList = bankRecipientList
                .GetRecipients(recipient => recipient.CanHandleLoanRequest(CreditScore, HistoryLength, LoanAmount));

            if (recipientList.Count == 0)
                recipientList.Add(catchAll.Queue);

            return recipientList.ToArray();
        }
    }
}
