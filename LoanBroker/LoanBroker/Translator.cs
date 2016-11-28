using Bank;
using LoanBroker.LoanBroker.Translators;
using CommonObjects;

namespace LoanBroker.LoanBroker
{
    internal class Translator

    {
        static LoanQuoteReplyTranslator _loanQuoteReplyTranslator = new LoanQuoteReplyTranslator();
        static BankQuoteRequestTranslator _bankQuoteRequestTranslator = new BankQuoteRequestTranslator();
        static CreditBureauRequestTranslator _CreditBureauRequestTranslator = new CreditBureauRequestTranslator();

        public static CreditBureauRequest GetCreditBureauRequest(LoanQuoteRequest loanRequest)
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
