using Bank;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Messaging.Base.Routing;

namespace LoanBroker.Bank
{
    internal class BankQuoteAggregate : Aggregate<int, BankQuoteReply, BankQuoteReply>
    {
        protected int expectedMessages;
        protected Object ACT;
        protected OnBestQuoteEvent callback;

        protected double bestRate = 0.0;

        protected ArrayList receivedMessages = new ArrayList();
        protected BankQuoteReply bestReply = null;

        public BankQuoteAggregate(int ID, int expectedMessages, OnBestQuoteEvent callback, Object ACT)
            : base(
            ID,
            (aggregatedValues => aggregatedValues.Count == expectedMessages)
            )
        {
            this.expectedMessages = expectedMessages;
            this.callback = callback;
            this.ACT = ACT;
        }

        public void AddMessage(BankQuoteReply reply)
        {
            if (reply.ErrorCode == 0)
            {
                if (bestReply == null)
                {
                    bestReply = reply;
                }
                else
                {
                    if (reply.InterestRate < bestReply.InterestRate)
                    {
                        bestReply = reply;
                    }
                }
            }

            this.AddValue(reply);
        }

        public BankQuoteReply getBestResult()
        {
            return bestReply;
        }

        public void NotifyBestResult()
        {
            if (callback != null)
            {
                callback(bestReply, ACT);
            }
        }
    }
}
