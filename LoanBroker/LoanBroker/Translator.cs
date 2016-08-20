using Bank;
using CreditBureau;
using LoanBroker.LoanBroker.Translators;
using LoanBroker.Models.LoanBroker;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoanBroker.LoanBroker
{
    internal class Translator

    {
        static LoanQuoteReplyTranslator _loanQuoteReplyTranslator = new LoanQuoteReplyTranslator();
        static BankQuoteRequestTranslator _bankQuoteRequestTranslator = new BankQuoteRequestTranslator();
        static CreditBureauRequestTranslator _CreditBureauRequestTranslator = new CreditBureauRequestTranslator();

        public static CreditBureauRequest GetCreditBureaurequest(LoanQuoteRequest loanRequest)
        {
            return _CreditBureauRequestTranslator.Translate(new CreditBureauRequestTranslator.Inputs
            {
                LoanRequest = loanRequest,
            });
        }

        public static BankQuoteRequest GetBankQuoteRequest(LoanQuoteRequest loanRequest, CreditBureauReply creditReply)
        {
            return _bankQuoteRequestTranslator.Translate(new BankQuoteRequestTranslator.Inputs
            {
                LoanRequest = loanRequest,
                CreditReply = creditReply
            });
        }

        public static LoanQuoteReply GetLoanQuoteReply(LoanQuoteRequest loanRequest, BankQuoteReply bestQuote)
        {
            return _loanQuoteReplyTranslator.Translate(new LoanQuoteReplyTranslator.Inputs
            {
                 LoanRequest = loanRequest,
                 BestQuote = bestQuote
            });
        }
    }
}
