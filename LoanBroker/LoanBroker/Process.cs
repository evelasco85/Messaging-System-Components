using Bank;
using CreditBureau;
using LoanBroker.CreditBureau;
using LoanBroker.Models.LoanBroker;
using Messaging.Base.Routing;
using System;
using System.Collections.Generic;
using System.Messaging;
using System.Text;

namespace LoanBroker.LoanBroker
{
    internal class Process
    {
        protected ProcessManager broker;
        protected String processID;
        protected LoanQuoteRequest loanRequest;
        protected Message message;

        protected ICreditBureauGateway creditBureauInterface;
        protected BankGateway bankInterface;

        public Process(ProcessManager broker, String processID,
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

            CreditBureauRequest creditRequest = Translator.GetCreditBureaurequest(loanRequest);
            creditBureauInterface.GetCreditScore(creditRequest, new OnCreditReplyEvent(OnCreditReply), null);
        }

        private void OnCreditReply(CreditBureauReply creditReply, Object act)
        {
            Console.WriteLine("Received Credit Score -- SSN {0} Score {1} Length {2}",
                creditReply.SSN, creditReply.CreditScore, creditReply.HistoryLength);
            BankQuoteRequest bankRequest = Translator.GetBankQuoteRequest(loanRequest, creditReply);
            bankInterface.GetBestQuote(bankRequest, new OnNotifyAggregationCompletion<BankQuoteReply>(OnBestQuote));
        }

        private void OnBestQuote(BankQuoteReply bestQuote)
        {
            LoanQuoteReply quoteReply = Translator.GetLoanQuoteReply(loanRequest, bestQuote);
            Console.WriteLine("Best quote {0} {1}", quoteReply.InterestRate, quoteReply.QuoteID);
            broker.SendReply(quoteReply, message);
            broker.OnProcessComplete(processID);
        }
    }
}
