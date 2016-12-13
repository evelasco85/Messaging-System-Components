using System;
using System.Collections;
using System.Threading;
using CommonObjects;
using Messaging.Base;
using Messaging.Base.Constructions;

namespace Test
{
    public class TestLoanBroker<TMessage>
    {
        protected IMessageSender<TMessage> _requestQueue;
        protected IMessageReceiver<TMessage> _replyQueue;

        protected int numMessages;

        protected IDictionary messageBuffer;
        protected IDictionary sentTimeBuffer;
        protected double AggregateResponseTime  = 0;
        protected Random random = new Random();
        protected DateTime startTime;

        IReturnAddress<TMessage> _loanBroakerReturnAddress;
        private Func<TMessage, Tuple<bool, LoanQuoteReply>> _extractLoanQuoteReplyFunc;
        private Func<object, Tuple<bool, LoanQuoteRequest>> _extractLoanQueueRequestFunc;


        public TestLoanBroker(
            IMessageSender<TMessage> requestQueue,
            IMessageReceiver<TMessage> replyQueue,
            int numMessages,
            Func<object, Tuple<bool, LoanQuoteRequest>> extractLoanQueueRequestFunc,
            Func<TMessage, Tuple<bool, LoanQuoteReply>> extractLoanQuoteReplyFunc
            )
        {
            _requestQueue = requestQueue;
            _replyQueue = replyQueue;
            _loanBroakerReturnAddress = _replyQueue.AsReturnAddress();

            _extractLoanQueueRequestFunc = extractLoanQueueRequestFunc;
            _extractLoanQuoteReplyFunc = extractLoanQuoteReplyFunc;

            _replyQueue.ReceiveMessageProcessor += new MessageDelegate<TMessage>(OnMessage);
            
            this.numMessages = numMessages;
            messageBuffer = new Hashtable();
            sentTimeBuffer = new Hashtable();

            Console.WriteLine("Sending {0} messages to {1}. Expecting replies on {2}", numMessages, requestQueue.QueueName, replyQueue.QueueName);
        }

        public void StopProcessing()
        {
            _replyQueue.StopReceivingMessages();
        }

        public void Process()
        {
            _replyQueue.StartReceivingMessages();
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

                TMessage msg = _requestQueue.Send(req, _loanBroakerReturnAddress,
                    (assignApplicationId, assignCorrelationId, assignPriority) =>
                    {
                        assignApplicationId(count.ToString());
                    });

                string messageId = _replyQueue.GetMessageId(msg);

                Console.WriteLine("Sent Request {0} {1:c} MsgID = {2}", req.SSN, req.LoanAmount, messageId);
                messageBuffer.Add(messageId, msg);
                sentTimeBuffer.Add(messageId, DateTime.Now);

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

                        Tuple<bool, LoanQuoteRequest> requestInfo = _extractLoanQueueRequestFunc(e.Value);
                        bool isLoanRequest = requestInfo.Item1;
                        LoanQuoteRequest request = requestInfo.Item2;

                        if (isLoanRequest)
                        {
                            Console.WriteLine("   {0} {1:c} {2}", request.SSN, request.LoanAmount, request.LoanTerm);
                        }
                    }
                }
            } 
 
        }

        private void OnMessage(TMessage msg)
        {
            Tuple<bool, LoanQuoteReply> replyInfo = _extractLoanQuoteReplyFunc(msg);

            bool isLoanQuoteReply = replyInfo.Item1;
            LoanQuoteReply reply = replyInfo.Item2;
            string correlationId = _replyQueue.GetMessageCorrelationId(msg);

            try
            {
                if (isLoanQuoteReply)
                {
                    Console.WriteLine("Received response: {0} {1:c} {2} {3}", reply.SSN, reply.LoanAmount, reply.InterestRate, reply.QuoteID);
                    if (messageBuffer.Contains(correlationId))
                    {
                        TMessage requestMsg = (TMessage)(messageBuffer[correlationId]);
                        DateTime sentTime = (DateTime)(sentTimeBuffer[correlationId]);
                        TimeSpan duration = DateTime.Now - sentTime;
                        AggregateResponseTime += duration.TotalSeconds;
                        Console.WriteLine("  Matched to request - {0:f} seconds", duration.TotalSeconds);
                        messageBuffer.Remove(correlationId);

                        if (messageBuffer.Count == 0)
                        {
                            Console.WriteLine("=== Total elapsed time: {0} secs", DateTime.Now - startTime);
                            Console.WriteLine("=== average response time {0:f} secs", AggregateResponseTime / numMessages);
                        }
                    }
                    else
                        Console.WriteLine("  UNMATCHED response: {0}", correlationId);
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
