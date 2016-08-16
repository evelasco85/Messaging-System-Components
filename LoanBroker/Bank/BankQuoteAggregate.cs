using Bank;
using System;
using System.Collections;
using System.Collections.Generic;
using Messaging.Base.Routing;

namespace LoanBroker.Bank
{
    internal class BankQuoteAggregate : Aggregate<int, BankQuoteReply, BankQuoteReply>
    {
        protected int expectedMessages;
        protected double bestRate = 0.0;

        protected ArrayList receivedMessages = new ArrayList();
        protected BankQuoteReply bestReply = null;

        public BankQuoteAggregate(int ID, int expectedMessages,
            OnNotifyAggregationCompletion<BankQuoteReply> onAggregationCompletion
            )
            : base(ID,
            (aggregatedValues => aggregatedValues.Count == expectedMessages),
            onAggregationCompletion
            )
        {
            this.expectedMessages = expectedMessages;
        }

        public override void PreAggregation(ref BankQuoteReply reply)
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
        }

        public override BankQuoteReply PerformAggregation(IList<BankQuoteReply> aggregatedValues)
        {
            return getBestResult();
        }

        BankQuoteReply getBestResult()
        {
            return bestReply;
        }
    }
}
