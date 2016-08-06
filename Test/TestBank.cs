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

namespace Test
{
    public class TestBank
    {

        protected IMessageSender<MessageQueue, Message>  requestQueue;
        protected MessageReceiverGateway replyQueue;

        protected int numMessages;

        private Random random = new Random();

        protected IDictionary messageIDs;

        protected IMessageFormatter GetFormatter()
        {
            return new XmlMessageFormatter(new Type[] {typeof(BankQuoteReply)});
        }

        public TestBank(String requestQueueName, String replyQueueName, int numMessages)
        {
            requestQueue = new MessageSenderGateway(requestQueueName);

            MessageReceiverGateway q = new MessageReceiverGateway(replyQueueName, GetFormatter());

            replyQueue = q;
            replyQueue.OnMessage += new OnMsgEvent<Message>(OnMessage);

            this.numMessages = numMessages;
            messageIDs = new Hashtable();

            Console.WriteLine("Sending {0} messages to {1}. Expecting replies on {2}", numMessages, requestQueueName, replyQueueName);
        }

        public void Process()
        {
            replyQueue.StartReceivingMessages();

            for (int count = 1; count <= numMessages; count++) 
            {
                BankQuoteRequest req = new BankQuoteRequest();
                req.SSN = count;
                req.LoanAmount =  random.Next(100)*10000 + 50000;
                req.LoanTerm = random.Next(36)+12;

                Message msg = new Message(req);
                msg.AppSpecific = count;
                msg.ResponseQueue = replyQueue.GetQueue();

                requestQueue.Send(msg);
                Console.WriteLine("Sent Request{0}  MsgID = {1}", req.SSN, msg.Id);
                messageIDs.Add(msg.Id, msg); 
            }
            Console.WriteLine();
            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }

        private void OnMessage(Message msg)
        {
            msg.Formatter = GetFormatter();
            try 
            {
                if (msg.Body is BankQuoteReply) 
                {
                    BankQuoteReply reply = (BankQuoteReply)msg.Body;
                    Console.WriteLine("Received response: {0} {1} {2}", reply.ErrorCode, reply.InterestRate, reply.QuoteID);
                    if (messageIDs.Contains(msg.CorrelationId))
                    {
                        Console.WriteLine("  Matched to request");
                        messageIDs.Remove(msg.CorrelationId);
                    }
                    else
                        Console.WriteLine("  UNMATCHED response: {0}", msg.CorrelationId);
                }
                else 
                {
                    Console.WriteLine("INVALID message received!!");
                }
            }
            catch (Exception e) 
            {
                Console.WriteLine("Exception: {0}", e.ToString());    
            }
        }

    }
}
