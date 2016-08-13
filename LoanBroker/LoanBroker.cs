/* Loan Broker Example with MSMQ
 * from Enterprise Integration Patterns (Addison-Wesley, ISBN 0321200683)
 * 
 * Copyright (c) 2003 Gregor Hohpe 
 *
 * This code is supplied as is. No warranties. 
 */

// The refactored loan broker that uses a separate process manager class.
// Adjusted to reference ICreditBureauGateay instead of a direct reference to the implementation class

using System;
using System.Messaging;
using System.Collections;
using MessageGateway;
using CreditBureau;
using Bank;

namespace LoanBroker
{
    public struct LoanQuoteRequest 
    {
        public int SSN;
        public double LoanAmount;
        public int LoanTerm;
    }

    public struct LoanQuoteReply
    {
        public int SSN;
        public double LoanAmount;  
        public double InterestRate;
        public string QuoteID;
    }

    internal class LoanBrokerProcess 
    {
        protected LoanBrokerPM broker;
        protected String processID;
        protected LoanQuoteRequest loanRequest;
        protected Message message;

        protected ICreditBureauGateway creditBureauInterface;
        protected BankGateway bankInterface;
    
        public LoanBrokerProcess(LoanBrokerPM broker, String processID, 
            ICreditBureauGateway creditBureauGateway,
            BankGateway bankGateway,
            LoanQuoteRequest loanRequest, Message msg) 
        {
            this.broker = broker;
            this.creditBureauInterface = creditBureauGateway;
            this.bankInterface = bankGateway;
            this.processID = processID;
            this.loanRequest = loanRequest;
            this.message = msg;

            CreditBureauRequest creditRequest = LoanBrokerTranslator.GetCreditBureaurequest(loanRequest);
            creditBureauInterface.GetCreditScore(creditRequest, new OnCreditReplyEvent(OnCreditReply), null);
        }

        private void OnCreditReply(CreditBureauReply creditReply, Object act)
        {
            Console.WriteLine("Received Credit Score -- SSN {0} Score {1} Length {2}",
                creditReply.SSN, creditReply.CreditScore, creditReply.HistoryLength);
            BankQuoteRequest bankRequest = LoanBrokerTranslator.GetBankQuoteRequest(loanRequest, creditReply);
            bankInterface.GetBestQuote(bankRequest, new OnBestQuoteEvent(OnBestQuote), null);
        }
        
        private void OnBestQuote(BankQuoteReply bestQuote, Object act)
        {
            LoanQuoteReply quoteReply = LoanBrokerTranslator.GetLoanQuoteReply(loanRequest, bestQuote);
            Console.WriteLine("Best quote {0} {1}", quoteReply.InterestRate, quoteReply.QuoteID);
            broker.SendReply(quoteReply, message);
            broker.OnProcessComplete(processID);
        }
    }

    internal class LoanBrokerPM : RequestReplyService_Asynchronous
    {
        protected ICreditBureauGateway creditBureauInterface;
        protected BankGateway bankInterface;
        protected IDictionary activeProcesses = (IDictionary)(new Hashtable());

        public LoanBrokerPM(String requestQueueName,
            String creditRequestQueueName, String creditReplyQueueName, 
            String bankReplyQueueName, BankConnectionManager connectionManager)
            : base(requestQueueName)
        {
            creditBureauInterface = (ICreditBureauGateway)
                (new CreditBureauGatewayImp(creditRequestQueueName, creditReplyQueueName));
            creditBureauInterface.Listen();

            bankInterface = new BankGateway(bankReplyQueueName, connectionManager);
            bankInterface.Listen();
        }

        public LoanBrokerPM(String requestQueueName, 
            ICreditBureauGateway creditBureau, 
            String bankReplyQueueName, BankConnectionManager connectionManager)
            : base(requestQueueName)
        {
            creditBureauInterface = creditBureau;
            creditBureauInterface.Listen();

            bankInterface = new BankGateway(bankReplyQueueName, connectionManager);
            bankInterface.Listen();
        }

        public override Type GetRequestBodyType()
        {
            return typeof(LoanQuoteRequest);
        }

        protected override void ProcessReceivedMessage(Object o, Message message)
        {
            LoanQuoteRequest quoteRequest;
            quoteRequest = (LoanQuoteRequest)o;

            String processID = message.Id;
            LoanBrokerProcess newProcess = 
                new LoanBrokerProcess(this, processID, creditBureauInterface,
                bankInterface, quoteRequest, message);
            activeProcesses.Add(processID, newProcess);
        }

        public void OnProcessComplete(String processID)
        {
            activeProcesses.Remove(processID);
        }
    }

    internal class LoanBrokerTranslator
    {
        public static CreditBureauRequest GetCreditBureaurequest(LoanQuoteRequest loanRequest)
        {
            CreditBureauRequest creditRequest = new CreditBureauRequest();
            creditRequest.SSN = loanRequest.SSN;
            return creditRequest;
        }

        public static BankQuoteRequest GetBankQuoteRequest(LoanQuoteRequest loanRequest, CreditBureauReply creditReply)
        {
            if (loanRequest.SSN != creditReply.SSN)
                return null;

            BankQuoteRequest bankRequest = new BankQuoteRequest();
            bankRequest.LoanAmount = System.Convert.ToInt32(loanRequest.LoanAmount);
            bankRequest.SSN = loanRequest.SSN;
            bankRequest.LoanTerm = loanRequest.LoanTerm;
            bankRequest.CreditScore = creditReply.CreditScore;
            bankRequest.HistoryLength = creditReply.HistoryLength;
            return bankRequest;
        }

        public static LoanQuoteReply GetLoanQuoteReply(LoanQuoteRequest loanRequest, BankQuoteReply bestQuote)
        {
        
            LoanQuoteReply quoteReply = new LoanQuoteReply();
            quoteReply.SSN = loanRequest.SSN;
            quoteReply.LoanAmount = Math.Floor(loanRequest.LoanAmount);
            if (bestQuote != null) 
            {
                quoteReply.InterestRate = bestQuote.InterestRate;
                quoteReply.QuoteID = bestQuote.QuoteID;
            }
            else 
            {
                quoteReply.InterestRate = 0.0;
                quoteReply.QuoteID = "ERROR: No Qualifying Quotes";
            }
            return quoteReply;
        }
    }

}
