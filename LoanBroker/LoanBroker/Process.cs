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
    internal class Process : Process<string, Process>
    {
        public override string GetKey()
        {
            return _processID;
        }

        public override Process GetProcessData()
        {
            return this;
        }

        String _processID;
        ProcessManager _broker;
        LoanQuoteRequest _loanRequest;
        Message _message;

        ICreditBureauGateway _creditBureauInterface;
        BankGateway _bankInterface;

        public Process(NotifyManagerDelegate<string, Process> managerNotifier,
            ProcessManager broker,
            String processID,
            ICreditBureauGateway creditBureauGateway,
            BankGateway bankGateway,
            LoanQuoteRequest loanRequest, Message msg)
            : base(managerNotifier)
        {
            this._broker = broker;
            this._creditBureauInterface = creditBureauGateway;
            this._bankInterface = bankGateway;
            this._processID = processID;
            this._loanRequest = loanRequest;
            this._message = msg;

            CreditBureauRequest creditRequest = Translator.GetCreditBureaurequest(loanRequest);
            _creditBureauInterface.GetCreditScore(creditRequest, new OnCreditReplyEvent(OnCreditReply), null);
        }

        private void OnCreditReply(CreditBureauReply creditReply, Object act)
        {
            Console.WriteLine("Received Credit Score -- SSN {0} Score {1} Length {2}",
                creditReply.SSN, creditReply.CreditScore, creditReply.HistoryLength);
            BankQuoteRequest bankRequest = Translator.GetBankQuoteRequest(_loanRequest, creditReply);
            _bankInterface.GetBestQuote(bankRequest, new OnNotifyAggregationCompletion<BankQuoteReply>(OnBestQuote));
        }

        private void OnBestQuote(BankQuoteReply bestQuote)
        {
            LoanQuoteReply quoteReply = Translator.GetLoanQuoteReply(_loanRequest, bestQuote);
            Console.WriteLine("Best quote {0} {1}", quoteReply.InterestRate, quoteReply.QuoteID);
            _broker.SendReply(quoteReply, _message);

            UpdateManager();
        }
    }
}
