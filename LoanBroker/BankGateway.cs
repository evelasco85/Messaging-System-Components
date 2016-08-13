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

namespace LoanBroker
{
    internal delegate void OnBestQuoteEvent(BankQuoteReply bestQuote, Object ACT);

    internal class BankGateway
    {
        protected IMessageReceiver<MessageQueue, Message> bankReplyQueue;
        protected ConnectionsManager connectionManager;

        protected IDictionary aggregateBuffer = (IDictionary)(new Hashtable());
        protected int aggregationCorrelationID;

        public BankGateway(String bankReplyQueueName, ConnectionsManager connectionManager)
        {
            MessageReceiverGateway receiver = 
                new MessageReceiverGateway(bankReplyQueueName, GetFormatter(), new MessageDelegate<Message>(OnBankMessage));
            this.bankReplyQueue = (IMessageReceiver<MessageQueue, Message>)receiver;
            this.connectionManager = connectionManager; 
            aggregationCorrelationID = 0;
        }

        protected IMessageFormatter GetFormatter()
        {
            return new XmlMessageFormatter(new Type[] {typeof(BankQuoteReply)});
        }

        public void Listen()
        {
            bankReplyQueue.StartReceivingMessages();
        }

        public void GetBestQuote(BankQuoteRequest quoteRequest, OnBestQuoteEvent onBestQuoteEvent, Object ACT)
        {

            Message requestMessage = new Message(quoteRequest);
            requestMessage.AppSpecific = aggregationCorrelationID;
            requestMessage.ResponseQueue = bankReplyQueue.GetQueue();
            IMessageSender<MessageQueue, Message> [] eligibleBanks = 
                connectionManager.GetEligibleBankQueues(quoteRequest.CreditScore, quoteRequest.HistoryLength, 
                quoteRequest.LoanAmount);
        
            aggregateBuffer.Add(aggregationCorrelationID, 
                new BankQuoteAggregate(aggregationCorrelationID, eligibleBanks.Length, onBestQuoteEvent, ACT));
            aggregationCorrelationID++;

            MessageRouter.SendToRecipientList(requestMessage, eligibleBanks);
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
                    if (aggregateBuffer.Contains(aggregationCorrelationID))
                    {
                        BankQuoteAggregate aggregate = 
                            (BankQuoteAggregate)(aggregateBuffer[aggregationCorrelationID]);
                        aggregate.AddMessage(replyStruct);
                        if (aggregate.IsComplete()) 
                        {
                            aggregate.NotifyBestResult();
                            aggregateBuffer.Remove(aggregationCorrelationID);
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
    }
}
