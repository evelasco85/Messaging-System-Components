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
using Messaging.Base.Constructions;

namespace Test
{
    public class TestBank
    {

        protected IMessageSender<MessageQueue, Message>  requestQueue;
        protected MessageReceiverGateway replyQueue;
        protected int numMessages;

        private Random random = new Random();
        IReturnAddress<Message> _bankReturnAddress;
        ICorrelationManager<string, Message> _correlationManager = new CorrelationManager<string, Message>();
        

        protected IMessageFormatter GetFormatter()
        {
            return new XmlMessageFormatter(new Type[] {typeof(BankQuoteReply)});
        }

        public TestBank(String requestQueueName, String replyQueueName, int numMessages)
        {
            requestQueue = new MessageSenderGateway(requestQueueName);

            MessageReceiverGateway q = new MessageReceiverGateway(replyQueueName, GetFormatter());

            replyQueue = q;
            replyQueue.ReceiveMessageProcessor += new MessageDelegate<Message>(OnMessage);
            _bankReturnAddress = new ReturnAddress<MessageQueue, Message>(replyQueue,
               (MessageQueue queue, ref Message message) =>
               {
                   message.ResponseQueue = queue;
               });

            this.numMessages = numMessages;

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

                _bankReturnAddress.SetMessageReturnAddress(ref msg);

                requestQueue.Send(msg);
                Console.WriteLine("Sent Request{0}  MsgID = {1}", req.SSN, msg.Id);
                _correlationManager.AddEntity(msg.Id, msg);
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

                    if (_correlationManager.EntityExists(msg.CorrelationId))
                    {
                        Console.WriteLine("  Matched to request");
                        _correlationManager.RemoveEntity(msg.CorrelationId);
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
