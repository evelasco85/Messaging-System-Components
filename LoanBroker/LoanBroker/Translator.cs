using Bank;
using CreditBureau;
using LoanBroker.Models.LoanBroker;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoanBroker.LoanBroker
{
    internal class Translator
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
