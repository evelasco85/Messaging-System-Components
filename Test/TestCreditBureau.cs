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
using MessageGateway;
using CreditBureau;
using Messaging.Base.Interface;

namespace Test
{
    public class TestCreditBureau
    {

        protected IMessageSender<MessageQueue, Message>  requestQueue;
        protected MessageReceiverGateway replyQueue;

        protected int numMessages;

        protected IDictionary messageIDs;

        protected Random random = new Random();

        protected IMessageFormatter GetFormatter()
        {
            return new XmlMessageFormatter(new Type[] {typeof(CreditBureauReply)});
        }

        public TestCreditBureau(String requestQueueName, String replyQueueName, int numMessages)
        {
            requestQueue = new MessageSenderGateway(requestQueueName);

            MessageReceiverGateway q = new MessageReceiverGateway(replyQueueName, GetFormatter());

            replyQueue = q;
            replyQueue.ReceiveMessageProcessor += new MessageDelegate<Message>(OnMessage);

            this.numMessages = numMessages;
            messageIDs = new Hashtable();

            Console.WriteLine("Sending {0} messages to {1}. Expecting replies on {2}", numMessages, requestQueueName, replyQueueName);
        }


        public void Process()
        {
            replyQueue.StartReceivingMessages();

            for (int count = 1; count <= numMessages; count++) 
            {
                CreditBureauRequest req = new CreditBureauRequest();
                req.SSN = count;

                Message msg = new Message(req);
                msg.AppSpecific = random.Next();
                msg.ResponseQueue = replyQueue.GetQueue();

                requestQueue.Send(msg);
                Console.WriteLine("Sent Request{0}  MsgID = {1}", req.SSN, msg.Id);
                messageIDs.Add(msg.AppSpecific, msg); 

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
                    if (messageIDs.Contains(msg.AppSpecific))
                    {
                        Console.WriteLine("  Matched to request");
                        messageIDs.Remove(msg.AppSpecific);
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
