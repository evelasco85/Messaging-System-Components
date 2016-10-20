using Bank;
using LoanBroker.Models.LoanBroker;
using Messaging.Base.Transformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonObjects;

namespace LoanBroker.LoanBroker.Translators
{
    public class BankQuoteRequestTranslator : Translator<BankQuoteRequest>
    {
        public class Inputs
        {
            public LoanQuoteRequest LoanRequest { get; set; }
            public CreditBureauReply CreditReply { get; set; }
        }

        public BankQuoteRequestTranslator()
        {
            TranslationDelegate<BankQuoteRequest, Inputs> translator = (input) =>
            {
                return GetBankQuoteRequest(input.LoanRequest, input.CreditReply);
            };

            RegisterTranslators(translator);
        }

        BankQuoteRequest GetBankQuoteRequest(LoanQuoteRequest loanRequest, CreditBureauReply creditReply)
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
    }
}
