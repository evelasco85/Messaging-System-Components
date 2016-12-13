using System;
using CommonObjects;
using Messaging.Base;

namespace Test
{
    class ClientInstance<TMessage>
    {
        TestLoanBroker<TMessage> test = null;

        public void SetupTestLoanBroker(
            IMessageSender<TMessage> sender,
            IMessageReceiver<TMessage> receiver,
            int numMessages,
            Func<object, Tuple<bool, LoanQuoteRequest>> extractLoanQueueRequestFunc,
            Func<TMessage, Tuple<bool, LoanQuoteReply>> extractLoanQuoteReplyFunc
            )
        {
            test = new TestLoanBroker<TMessage>(
                sender,
                receiver,
                numMessages,
                extractLoanQueueRequestFunc,
                extractLoanQuoteReplyFunc
                );
        }

        public void StartProcessing()
        {
            if (test != null)
            {
                test.Process();
                Console.WriteLine("Starting Application!");
            }
        }

        public void StopProcessing()
        {
            if (test != null)
            {
                test.StopProcessing();
                Console.WriteLine("Stopping Application!");
            }
        }
    }
}
