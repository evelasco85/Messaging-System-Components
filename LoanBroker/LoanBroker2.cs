// Enterprise Integration Patters: LoanBroker3.cs
//
// The refactored loan broker that uses a separate process manager class.
//
using System;
using System.Messaging;
using System.Collections;
using MessageGateway;
using CreditBureau;
using Bank;

namespace LoanBroker
{
// [[BEGIN]]
internal class LoanBrokerProcess 
{
    protected LoanBrokerPM broker;
    protected String processID;
    protected LoanQuoteRequest loanRequest;
    protected Message message;

    protected CreditBureauGateway creditBureauGateway;
    protected BankGateway bankInterface;

    public LoanBrokerProcess(LoanBrokerPM broker, String processID, 
                             CreditBureauGateway creditBureauGateway,
                             BankGateway bankGateway,
                             LoanQuoteRequest loanRequest, Message msg) 
    {
        this.broker = broker;
        this.creditBureauGateway = creditBureauGateway;
        this.bankInterface = bankGateway;
        this.processID = processID;
        this.loanRequest = loanRequest;
        this.message = msg;

        CreditBureauRequest creditRequest = LoanBrokerTranslator.GetCreditBureaurequest(loanRequest);
        creditBureauGateway.GetCreditScore(creditRequest, new OnCreditReplyEvent(OnCreditReply), null);
    }

    private void OnCreditReply(CreditBureauReply creditReply, Object act)
    {
        Console.WriteLine("Received Credit Score -- SSN {0} Score {1} Length {2}", creditReply.SSN, creditReply.CreditScore, creditReply.HistoryLength);
        BankQuoteRequest bankRequest = LoanBrokerTranslator.GetBankQuoteRequest(loanRequest, creditReply);
        bankInterface.GetBestQuote(bankRequest, new OnBestQuoteEvent(OnBestQuote), null);
    }
        
    private void OnBestQuote(BankQuoteReply bestQuote, Object act)
    {
        LoanQuoteReply quoteReply = LoanBrokerTranslator.GetLoanQuoteReply(loanRequest, bestQuote);
        Console.WriteLine("Best quote {0} {1}", quoteReply.InterestRate, quoteReply.QuoteID);
        broker.SendReply(quoteReply, message);
        broker.OnProcessComplete(processID);
    }
}
// [[END]]
// [[BEGIN]]
internal class LoanBrokerPM : AsyncRequestReplyService
{
    protected CreditBureauGateway creditBureauGateway;
    protected BankGateway bankInterface;
    protected IDictionary activeProcesses = (IDictionary)(new Hashtable());
    
    public LoanBrokerPM(String requestQueueName,
                        String creditRequestQueueName, String creditReplyQueueName, 
                        String bankReplyQueueName, BankConnectionManager connectionManager): base(requestQueueName)
    {
        creditBureauGateway = new CreditBureauGateway(creditRequestQueueName, creditReplyQueueName);
        creditBureauGateway.Listen();

        bankInterface = new BankGateway(bankReplyQueueName, connectionManager);
        bankInterface.Listen();
    }

    protected override Type GetRequestBodyType()
    {
        return typeof(LoanQuoteRequest);
    }

    protected override void ProcessMessage(Object o, Message message)
    {
        LoanQuoteRequest quoteRequest;
        quoteRequest = (LoanQuoteRequest)o;

        String processID = message.Id;
        LoanBrokerProcess newProcess = 
            new LoanBrokerProcess(this, processID, creditBureauGateway,
                                  bankInterface, quoteRequest, message);
        activeProcesses.Add(processID, newProcess);
    }

    public void OnProcessComplete(String processID)
    {
        activeProcesses.Remove(processID);
    }
}
// [[END]]
}
