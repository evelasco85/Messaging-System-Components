using System;
using CommonObjects;
using Messaging.Base;

namespace Test
{
    class ClientInstance<TMessage>
    {
        
        TestLoanBroker<TMessage> test = null;
        private IMessageSender<TMessage> _sender;
        private IMessageReceiver<TMessage> _receiver;

        public void SetupTestLoanBroker(
            IMessageSender<TMessage> sender,
            IMessageReceiver<TMessage> receiver,
            int numMessages,
            Func<TMessage, string> extractMessageIdFunc,
            Func<int, LoanQuoteRequest, TMessage> createLoanRequestMessageFunc,
            Func<object, Tuple<bool, LoanQuoteRequest>> extractLoanQueueRequestFunc,
            Func<TMessage, Tuple<string, bool, LoanQuoteReply>> extractLoanQuoteReplyFunc
            )
        {
            test = new TestLoanBroker<TMessage>(
                sender,
                receiver,
                numMessages,
                extractMessageIdFunc,
                createLoanRequestMessageFunc,
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
