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

namespace LoanBroker
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
    internal class BankConnectionManager
    {
        static protected BankConnection[] banks = {new Bank1(), new Bank2(), new Bank3(), new Bank4() };
        static protected BankConnection catchAll = new Bank5();

        public IMessageSender<MessageQueue, Message> [] GetEligibleBankQueues(int CreditScore, int HistoryLength, int LoanAmount)
        {
            ArrayList lenders = new ArrayList();

            for (int index = 0; index < banks.Length; index++) 
            {
                if (banks[index].CanHandleLoanRequest(CreditScore, HistoryLength, LoanAmount))
                    lenders.Add(banks[index].Queue);
            }
            if (lenders.Count == 0)
                lenders.Add(catchAll.Queue);
            IMessageSender<MessageQueue, Message> [] lenderArray = (IMessageSender<MessageQueue, Message>  [])Array.CreateInstance(typeof(IMessageSender<MessageQueue, Message> ), lenders.Count);
            lenders.CopyTo(lenderArray);
            return lenderArray;
        }
    }

    internal abstract class BankConnection
    {
        protected MessageSenderGateway queue;
        protected String bankName = "";
        public MessageSenderGateway Queue
        {
            get { return queue; }
        }
        public String BankName 
        {
            get { return bankName; }
        }
        public BankConnection (MessageQueue queue) { this.queue = new MessageSenderGateway(queue); }
        public BankConnection (String queueName) { this.queue = new MessageSenderGateway(queueName); }

        public abstract bool CanHandleLoanRequest(int CreditScore, int HistoryLength, int LoanAmount);
    }

    internal class Bank1 : BankConnection
    {  
        protected String bankname = "Exclusive Country Club Bankers";
    
        public Bank1 () : base (".\\private$\\bank1Queue") {}
        public override bool CanHandleLoanRequest(int CreditScore, int HistoryLength, int LoanAmount)
        {
            return LoanAmount >= 75000 && CreditScore >= 600 && HistoryLength >= 8;
        }
    }

    internal class Bank2 : BankConnection
    {  
        protected String bankname = "Acme Deluxe Bankers";
        
        public Bank2 () : base (".\\private$\\bank2Queue") {}
        public override bool CanHandleLoanRequest(int CreditScore, int HistoryLength, int LoanAmount)
        {
            return LoanAmount >= 75000 && CreditScore >= 600 && HistoryLength >= 8;
        }
    }

    internal class Bank3 : BankConnection
    {  
        protected String bankname = "General Retail Bankers";
        
        public Bank3 () : base (".\\private$\\bank3Queue") {}
        public override bool CanHandleLoanRequest(int CreditScore, int HistoryLength, int LoanAmount)
        {
            return LoanAmount >= 10000 && LoanAmount < 75000 && CreditScore >= 400 && HistoryLength >= 3;
        }
    }
   
    internal class Bank4 : BankConnection
    {  
        protected String bankname = "Neighborhood Bankers";
        
        public Bank4 () : base (".\\private$\\bank4Queue") {}
        public override bool CanHandleLoanRequest(int CreditScore, int HistoryLength, int LoanAmount)
        {
            return LoanAmount >= 10000 && LoanAmount < 75000 && CreditScore >= 400 && HistoryLength >= 3;
        }
    }

    internal class Bank5 : BankConnection
    {  
        protected String bankname = "Mom and Pop Generic Loan Sharks and Pawn Shop";
        
        public Bank5 () : base (".\\private$\\bank5Queue") {}
        public override bool CanHandleLoanRequest(int CreditScore, int HistoryLength, int LoanAmount)
        {
            return true;
        }
    }
}
