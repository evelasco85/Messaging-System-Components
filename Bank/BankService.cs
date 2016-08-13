/* Loan Broker Example with MSMQ
 * from Enterprise Integration Patterns (Addison-Wesley, ISBN 0321200683)
 * 
 * Copyright (c) 2003 Gregor Hohpe 
 *
 * This code is supplied as is. No warranties. 
 */

using System;
using System.Messaging;
using System.Threading;
using MessageGateway;

namespace Bank
{

    public struct BankQuoteRequest 
    {
        public int SSN;
        public int CreditScore;
        public int HistoryLength;
        public int LoanAmount;
        public int LoanTerm;
    }

    public struct BankQuoteReply
    {
        public double InterestRate;
        public String QuoteID;
        public int ErrorCode;
    }

    internal class Bank : RequestReplyService_Synchronous
    {
        private readonly double PrimeRate = 3.5;

        private String bankName;
        private double ratePremium;
        private int maxLoanTerm;

        private int quoteCounter = 0;

        protected Random random = new Random();

        public String BankName 
        { 
            get { return bankName; }
            set { bankName = value; }
        }
        
        public double RatePremium
        {
            get {return ratePremium; }
            set 
            { 
                if (value >0) 
                    ratePremium = value; 
                else 
                    ratePremium = 0.0; 
            }
        }

        public int MaxLoanTerm
        {
            get { return maxLoanTerm; }
            set 
            {
                if (value > 0)
                    maxLoanTerm = value;
                else
                    maxLoanTerm = 0;
            }
        }

        public Bank(String requestQueue) : base(requestQueue)
        {
        }   

        public Bank(String requestQueue, String bankName, double ratePremium, int maxLoanTerm) 
            : base(requestQueue)
        {
            BankName = bankName;
            RatePremium = ratePremium;
            MaxLoanTerm = maxLoanTerm;
        }   

        public override Type GetRequestBodyType()
        {
            return typeof(BankQuoteRequest);
        }

        protected  BankQuoteReply ComputeBankReply(BankQuoteRequest requestStruct)
        {
            BankQuoteReply replyStruct = new BankQuoteReply();

            if (requestStruct.LoanTerm <= MaxLoanTerm) 
            {
                replyStruct.InterestRate = PrimeRate + RatePremium 
                    + (double)(requestStruct.LoanTerm / 12)/10 
                    + (double)random.Next(10) / 10;
                replyStruct.ErrorCode = 0;
            }
            else
            {
                replyStruct.InterestRate = 0.0;
                replyStruct.ErrorCode = 1;
            }
            replyStruct.QuoteID = String.Format("{0}-{1:00000}", BankName, quoteCounter);
            quoteCounter++;
            return replyStruct;
        }

        protected override Object ProcessReceivedMessage(Object o)
        {
            BankQuoteRequest requestStruct;
            BankQuoteReply replyStruct;

            requestStruct = (BankQuoteRequest)o;
            replyStruct = ComputeBankReply(requestStruct);

            Console.WriteLine("Received request for SSN {0} for {1:c} / {2} months", 
                requestStruct.SSN, requestStruct.LoanAmount, requestStruct.LoanTerm);
            Thread.Sleep(random.Next(10) * 100);
            Console.WriteLine("  Quote: {0} {1} {2}", 
                replyStruct.ErrorCode, replyStruct.InterestRate, replyStruct.QuoteID); 

            return replyStruct;
        }
    }
}
