﻿using Bank;
using LoanBroker.Models.LoanBroker;
using Messaging.Base.Transformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanBroker.LoanBroker.Translators
{
    public class LoanQuoteReplyTranslator : Translator<LoanQuoteReply>
    {
        public class Inputs
        {
            public LoanQuoteRequest LoanRequest { get; set; }
            public BankQuoteReply BestQuote { get; set; }
        }

        public LoanQuoteReplyTranslator()
        {
            TranslationDelegate<LoanQuoteReply, Inputs> translator = (input) =>
            {
                return GetLoanQuoteReply(input.LoanRequest, input.BestQuote);
            };

            RegisterTranslators(translator);
        }

        LoanQuoteReply GetLoanQuoteReply(LoanQuoteRequest loanRequest, BankQuoteReply bestQuote)
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
