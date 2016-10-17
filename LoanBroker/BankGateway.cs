/* Loan Broker Example with MSMQ
 * from Enterprise Integration Patterns (Addison-Wesley, ISBN 0321200683)
 * 
 * Copyright (c) 2003 Gregor Hohpe 
 *
 * This code is supplied as is. No warranties. 
 */

using System;
using System.Messaging;
using System.Collections;
using MessageGateway;
using Bank;
using Messaging.Base;
using LoanBroker.Bank;
using Messaging.Base.Routing;
using Messaging.Base.Constructions;
using MsmqGateway;

namespace LoanBroker
{
    internal class BankGateway
    {
        protected IMessageReceiver<MessageQueue, Message> bankReplyQueue;
        protected ConnectionsManager connectionManager;

        IAggregator<int, BankQuoteReply, BankQuoteReply, BankQuoteAggregate> _aggregator = new Aggregator<int, BankQuoteReply, BankQuoteReply, BankQuoteAggregate>();

        protected int aggregationCorrelationID;
        IReturnAddress<Message> _bankReturnAddress;

        public BankGateway(String bankReplyQueueName, ConnectionsManager connectionManager)
        {
            MessageReceiverGateway receiver = 
                new MessageReceiverGateway(bankReplyQueueName, GetFormatter(), new MessageDelegate<Message>(OnBankMessage));
            this.bankReplyQueue = (IMessageReceiver<MessageQueue, Message>)receiver;
            this.connectionManager = connectionManager; 
            aggregationCorrelationID = 0;

            _bankReturnAddress = new MQReturnAddress(bankReplyQueue);
        }

        public static IMessageFormatter GetFormatter()
        {
            return new XmlMessageFormatter(new Type[] {typeof(BankQuoteReply)});
        }

        public void Listen()
        {
            bankReplyQueue.StartReceivingMessages();
        }

        public void GetBestQuote(BankQuoteRequest quoteRequest, OnNotifyAggregationCompletion<BankQuoteReply> onBestQuoteEvent)
        {
            Message requestMessage = new Message(quoteRequest);

            _bankReturnAddress.SetMessageReturnAddress(ref requestMessage);

            requestMessage.AppSpecific = aggregationCorrelationID;
            
            IMessageSender<MessageQueue, Message> [] eligibleBanks = 
                connectionManager.GetEligibleBankQueues(quoteRequest.CreditScore, quoteRequest.HistoryLength, 
                quoteRequest.LoanAmount);

            _aggregator.AddAggregate(aggregationCorrelationID, new BankQuoteAggregate(aggregationCorrelationID, eligibleBanks.Length, onBestQuoteEvent));
            aggregationCorrelationID++;

            MessageRouter
                .GetInstance()
                .SendToRecipent(requestMessage, eligibleBanks);
        }


        private void OnBankMessage(Message msg)
        {
            msg.Formatter =  GetFormatter();

            BankQuoteReply replyStruct;

            try 
            {
                if (msg.Body is BankQuoteReply) 
                {
                    replyStruct = (BankQuoteReply)msg.Body;
                    int aggregationCorrelationID = msg.AppSpecific;
                    Console.WriteLine("Quote {0:0.00}% {1} {2}", 
                        replyStruct.InterestRate, replyStruct.QuoteID, replyStruct.ErrorCode);
                    if (_aggregator.Contains(aggregationCorrelationID))
                    {
                        BankQuoteAggregate aggregate = _aggregator.GetAggregate(aggregationCorrelationID);

                        aggregate.AggregateValue(replyStruct);

                        if (aggregate.IsComplete()) 
                        {
                            aggregate.NotifyAggregationCompletion();
                            _aggregator.RemoveAggregate(aggregate);
                        }
                    }
                    else 
                    { Console.WriteLine("Incoming bank response does not match any aggregate"); }
                }
                else
                { Console.WriteLine("Illegal request."); }
            }
            catch (Exception e) 
            {
                Console.WriteLine("Exception: {0}", e.ToString());    
            }
        }

        public int GetOutstandingAggregateCount()
        {
            return _aggregator.GetAggregateCount();
        }
    }
}
