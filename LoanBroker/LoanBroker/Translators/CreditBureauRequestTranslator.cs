using Messaging.Base.Transformation;
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
