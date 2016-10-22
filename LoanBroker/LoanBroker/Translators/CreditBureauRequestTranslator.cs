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
    public class CreditBureauRequestTranslator : Translator<CreditBureauRequest>
    {
        public class Inputs
        {
            public LoanQuoteRequest LoanRequest { get; set; }
        }

        public CreditBureauRequestTranslator()
        {
            TranslationDelegate<CreditBureauRequest, Inputs> translator = (input) =>
            {
                return GetCreditBureaurequest(input.LoanRequest);
            };

            RegisterTranslator(translator);
        }

        CreditBureauRequest GetCreditBureaurequest(LoanQuoteRequest loanRequest)
        {
            CreditBureauRequest creditRequest = new CreditBureauRequest();

            creditRequest.SSN = loanRequest.SSN;

            return creditRequest;
        }
    }
}
