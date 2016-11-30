using CommonObjects;

namespace LoanBroker.CreditBureau
{
    public delegate void OnCreditReplyEvent(CreditBureauReply creditReply);

    internal struct CreditRequestProcess
    {
        public OnCreditReplyEvent callback;
    }
}
