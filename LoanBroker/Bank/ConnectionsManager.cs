using System;
using Messaging.Base;
using Messaging.Base.Routing;
using System.Linq;
using System.Collections.Generic;
using MsmqGateway.Core;

namespace LoanBroker.Bank
{
    // Modified connection manager that sends requests to Bank 5 only if there are not other takers
    internal class ConnectionsManager<TMessage>
    {
        static protected IRecipientList<Connection<TMessage>, TMessage> bankRecipientList = new RecipientList<Connection<TMessage>, TMessage>(
            (recipient => recipient.Queue),
            new Bank1<TMessage>(CreateSender("bank1Queue")),
            new Bank2<TMessage>(CreateSender("bank2Queue")),
            new Bank3<TMessage>(CreateSender("bank3Queue")),
            new Bank4<TMessage>(CreateSender("bank4Queue"))
            );
        static protected Connection<TMessage> catchAll = new Bank5<TMessage>(CreateSender("bank5Queue"));

        public IMessageSender<TMessage>[] GetEligibleBankQueues(int CreditScore, int HistoryLength, int LoanAmount)
        {
            IList<IMessageSender<TMessage>> recipientList = bankRecipientList
                .GetRecipients(recipient => recipient.CanHandleLoanRequest(CreditScore, HistoryLength, LoanAmount));

            if (recipientList.Count == 0)
                recipientList.Add(catchAll.Queue);

            return recipientList.ToArray();
        }

        static String ToPath(String arg)
        {
            return ".\\private$\\" + arg;
        }

        public static IMessageSender<TMessage> CreateSender(string queueName)
        {
            return (IMessageSender<TMessage>) new MessageSenderGateway(ToPath(queueName));
        }
    }
}
