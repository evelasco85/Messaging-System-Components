using System;
using System.Collections.Generic;
using System.Messaging;
using System.Threading;
using CreditBureau;
using MsmqGateway.Core;
using Messaging.Base;
using Messaging.Base.Constructions;

namespace Test
{
    public class TestCreditBureau
    {

        protected IMessageSender<MessageQueue, Message>  requestQueue;
        protected MessageReceiverGateway<CreditBureauReply> replyQueue;
        protected int numMessages;
        protected Random random = new Random();

        IReturnAddress<Message> _creditReturnAddress;
        ICorrelationManager<int, Message> _correlationManager = new CorrelationManager<int, Message>();

        protected IMessageFormatter GetFormatter()
        {
            return new XmlMessageFormatter(new Type[] {typeof(CreditBureauReply)});
        }

        public TestCreditBureau(String requestQueueName, String replyQueueName, int numMessages)
        {
            requestQueue = new MessageSenderGateway(requestQueueName);
            replyQueue = new MessageReceiverGateway<CreditBureauReply>(replyQueueName);

            replyQueue.ReceiveMessageProcessor += new MessageDelegate<Message>(OnMessage);
            _creditReturnAddress = replyQueue.AsReturnAddress();

            this.numMessages = numMessages;

            Console.WriteLine("Sending {0} messages to {1}. Expecting replies on {2}", numMessages, requestQueueName, replyQueueName);
        }


        public void Process()
        {
            replyQueue.StartReceivingMessages();

            for (int count = 1; count <= numMessages; count++)
            {
                CreditBureauRequest req = new CreditBureauRequest()
                {
                    SSN = count
                };
                Message msg = requestQueue.Send(req, _creditReturnAddress,
                    assignProperty =>
                    {
                        assignProperty("AppSpecific", random.Next());
                    });

                Console.WriteLine("Sent Request{0}  MsgID = {1}", req.SSN, msg.Id);

                _correlationManager.AddEntity(msg.AppSpecific, msg);

                Thread.Sleep(200);
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
                if (msg.Body is CreditBureauReply) 
                {
                    CreditBureauReply reply = (CreditBureauReply)msg.Body;
                    Console.WriteLine("Received response: {0} {1} {2}", reply.SSN, reply.CreditScore, reply.HistoryLength);

                    if (_correlationManager.EntityExists(msg.AppSpecific))
                    {
                        Console.WriteLine("  Matched to request");
                        _correlationManager.RemoveEntity(msg.AppSpecific);
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
