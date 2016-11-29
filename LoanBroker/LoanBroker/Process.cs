using Bank;
using LoanBroker.CreditBureau;
using Messaging.Base.Routing;
using System;
using CommonObjects;

namespace LoanBroker.LoanBroker
{
    internal class Process<TMessage> : Process<string, Process<TMessage>, ProcessManager<TMessage>>
    {
        public override string GetKey()
        {
            return _processID;
        }

        public override Process<TMessage> GetProcessData()
        {
            return this;
        }

        String _processID;
        LoanQuoteRequest _loanRequest;
        TMessage _originalMessage;

        public Process(String processID, LoanQuoteRequest loanRequest, TMessage originalMessage) 
        {
            this._processID = processID;
            this._loanRequest = loanRequest;
            this._originalMessage = originalMessage;
        }

        public override void StartProcess(ProcessManager<TMessage> processor)
        {
            CreditBureauRequest creditRequest = Translator.GetCreditBureauRequest(_loanRequest);

            processor.CreditBureauInterface.GetCreditScore(creditRequest, new OnCreditReplyEvent(OnCreditReply));
        }

        private void OnCreditReply(CreditBureauReply creditReply)
        {
            Console.WriteLine("Received Credit Score -- SSN {0} Score {1} Length {2}",
                creditReply.SSN, creditReply.CreditScore, creditReply.HistoryLength);
            BankQuoteRequest bankRequest = Translator.GetBankQuoteRequest(_loanRequest, creditReply);

            this.GetProcessor().BankInterface.GetBestQuote(bankRequest, new OnNotifyAggregationCompletion<BankQuoteReply>(OnBestQuote));
        }

        private void OnBestQuote(BankQuoteReply bestQuote)
        {
            LoanQuoteReply quoteReply = Translator.GetLoanQuoteReply(_loanRequest, bestQuote);
            Console.WriteLine("Best quote {0} {1}", quoteReply.InterestRate, quoteReply.QuoteID);

            this.GetProcessor().SendReply(quoteReply, _originalMessage);

            UpdateManager();
        }
    }
}
