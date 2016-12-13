using System;
using CommonObjects;
using Messaging.Base;

namespace Test
{
    class ClientInstance
    {
        static SenderPropertyFields s_senderPropertyFields = new SenderPropertyFields();

        public static SenderPropertyFields PropertyFields
        {
            get { return s_senderPropertyFields; }
            set { s_senderPropertyFields = value; }
        }
    }

    class ClientInstance<TMessage>
    {
        TestLoanBroker<TMessage> test = null;

        public void SetupTestLoanBroker(
            IMessageSender<TMessage> sender,
            IMessageReceiver<TMessage> receiver,
            int numMessages,
            Func<TMessage, string> extractMessageIdFunc,
            Func<object, Tuple<bool, LoanQuoteRequest>> extractLoanQueueRequestFunc,
            Func<TMessage, Tuple<string, bool, LoanQuoteReply>> extractLoanQuoteReplyFunc
            )
        {
            test = new TestLoanBroker<TMessage>(
                sender,
                receiver,
                numMessages,
                extractMessageIdFunc,
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
