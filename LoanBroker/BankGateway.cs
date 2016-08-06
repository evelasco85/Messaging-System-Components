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
using Messaging.Base.Interface;

namespace LoanBroker
{
    internal delegate void OnBestQuoteEvent(BankQuoteReply bestQuote, Object ACT);

    internal class MessageRouter
    {
        public static void SendToRecipientList (Message msg, IMessageSender<MessageQueue, Message> [] recipientList)
        {   
            IEnumerator e = recipientList.GetEnumerator();
            while (e.MoveNext()) 
            {
                ((IMessageSender<MessageQueue, Message> )e.Current).Send(msg);
            }
        }
    }

    internal class BankQuoteAggregate
    {
        protected int ID;
        protected int expectedMessages;
        protected Object ACT;
        protected OnBestQuoteEvent callback;

        protected double bestRate = 0.0;

        protected ArrayList receivedMessages = new ArrayList();
        protected BankQuoteReply bestReply = null;

        public BankQuoteAggregate(int ID, int expectedMessages, OnBestQuoteEvent callback, Object ACT)
        {
            this.ID = ID;
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
            receivedMessages.Add(reply);
        }

        public bool IsComplete()
        {
            return receivedMessages.Count == expectedMessages;
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

    internal class BankGateway
    {
        protected IMessageReceiver<MessageQueue, Message> bankReplyQueue;
        protected BankConnectionManager connectionManager;

        protected IDictionary aggregateBuffer = (IDictionary)(new Hashtable());
        protected int aggregationCorrelationID;

        public BankGateway(String bankReplyQueueName, BankConnectionManager connectionManager)
        {
            MessageReceiverGateway receiver = 
                new MessageReceiverGateway(bankReplyQueueName, GetFormatter(), new OnMsgEvent<Message>(OnBankMessage));
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
