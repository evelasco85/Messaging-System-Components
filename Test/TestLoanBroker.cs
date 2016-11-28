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
using System.Threading;
using CommonObjects;
using MessageGateway;
using Messaging.Base;
using Messaging.Base.Constructions;

namespace Test
{

    public class TestLoanBroker
    {
        protected IMessageSender<MessageQueue, Message> requestQueue;
        protected MessageReceiverGateway replyQueue;

        protected int numMessages;

        protected IDictionary messageBuffer;
        protected IDictionary sentTimeBuffer;
        protected double AggregateResponseTime  = 0;
        protected Random random = new Random();
        protected DateTime startTime;

        IReturnAddress<Message> _loanBroakerReturnAddress;

        protected IMessageFormatter GetFormatter()
        {
            return new XmlMessageFormatter(new Type[] {typeof(LoanQuoteReply)});
        }

        public TestLoanBroker(String requestQueueName, String replyQueueName, int numMessages)
        {
            requestQueue = new MessageSenderGateway(requestQueueName);
            replyQueue = new MessageReceiverGateway(replyQueueName, GetFormatter());
            replyQueue.ReceiveMessageProcessor += new MessageDelegate<Message>(OnMessage);

            _loanBroakerReturnAddress = replyQueue.AsReturnAddress();

            
            this.numMessages = numMessages;
            messageBuffer = new Hashtable();
            sentTimeBuffer = new Hashtable();

            Console.WriteLine("Sending {0} messages to {1}. Expecting replies on {2}", numMessages, requestQueueName, replyQueueName);
        }

        public void StopProcessing()
        {
            replyQueue.StopReceivingMessages();
        }

        public void Process()
        {
            replyQueue.StartReceivingMessages();
            startTime = DateTime.Now;

            Func<int, bool> loopCondition = (currentCount) =>
            {
                return (numMessages == 0) ? true : (currentCount <= this.numMessages);
            };

            for (int count = 1; loopCondition(count); count++)
            {
                LoanQuoteRequest req = new LoanQuoteRequest();
                req.SSN = count;
                req.LoanAmount = random.Next(20) * 5000 + 25000;
                req.LoanTerm = random.Next(72) + 12;

                Message msg = new Message(req);
                msg.AppSpecific = count;

                _loanBroakerReturnAddress.SetMessageReturnAddress(ref msg);

                requestQueue.Send(msg);
                Console.WriteLine("Sent Request {0} {1:c} MsgID = {2}", req.SSN, req.LoanAmount, msg.Id);
                messageBuffer.Add(msg.Id, msg);
                sentTimeBuffer.Add(msg.Id, DateTime.Now);

                Thread.Sleep(100);
            }
            Console.WriteLine();
            Console.WriteLine("Press 'n' to show number of outstanding messages.");
            Console.WriteLine("Press 'l' to list all outstanding messages.");
            Console.WriteLine("Press Enter to exit.");

            for (; ; )
            {
                String s = Console.ReadLine();
                if (s.Length == 0)
                    break;
                char c = s[0];
                if (c == 'n')
                {
                    Console.WriteLine("{0} messages outstanding", messageBuffer.Count);
                }
                else if (c == 'l')
                {
                    IDictionaryEnumerator e = messageBuffer.GetEnumerator();
                    while (e.MoveNext())
                    {
                        Console.WriteLine("{0}", e.Key);
                        Message msg = (Message)e.Value;
                        if (msg.Body is LoanQuoteRequest)
                        {
                            LoanQuoteRequest req = (LoanQuoteRequest)msg.Body;
                            Console.WriteLine("   {0} {1:c} {2}", req.SSN, req.LoanAmount, req.LoanTerm);
                        }
                    }
                }
            } 
 
        }

        private void OnMessage(Message msg)
        {
            msg.Formatter = GetFormatter();
            try
            {
                if (msg.Body is LoanQuoteReply)
                {
                    LoanQuoteReply reply = (LoanQuoteReply)msg.Body;
                    Console.WriteLine("Received response: {0} {1:c} {2} {3}", reply.SSN, reply.LoanAmount, reply.InterestRate, reply.QuoteID);
                    if (messageBuffer.Contains(msg.CorrelationId))
                    {
                        Message requestMsg = (Message)(messageBuffer[msg.CorrelationId]);
                        DateTime sentTime = (DateTime)(sentTimeBuffer[msg.CorrelationId]);
                        TimeSpan duration = DateTime.Now - sentTime;
                        AggregateResponseTime += duration.TotalSeconds;
                        Console.WriteLine("  Matched to request - {0:f} seconds", duration.TotalSeconds);
                        messageBuffer.Remove(msg.CorrelationId);

                        if (messageBuffer.Count == 0)
                        {
                            Console.WriteLine("=== Total elapsed time: {0} secs", DateTime.Now - startTime);
                            Console.WriteLine("=== average response time {0:f} secs", AggregateResponseTime / numMessages);
                        }
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
