/* Loan Broker Example with MSMQ
 * from Enterprise Integration Patterns (Addison-Wesley, ISBN 0321200683)
 * 
 * Copyright (c) 2003 Gregor Hohpe 
 *
 * This code is supplied as is. No warranties. 
 */

using Messaging.Base;
using Messaging.Base.Interface;
using System;
using System.Messaging;

namespace MessageGateway
{
    public abstract class MQService : QueueService<MessageQueue, Message>
    {
        static protected readonly String InvalidMessageQueueName = ".\\private$\\invalidMessageQueue";
        IMessageSender<MessageQueue, Message>invalidQueue = new MessageSenderGateway(InvalidMessageQueueName);

        protected IMessageReceiver<MessageQueue, Message> requestQueue;
        protected Type requestBodyType;
	
        public MQService(IMessageReceiver<MessageQueue, Message> receiver)
        {
            requestQueue = receiver;

            RegisterReceiver(requestQueue);
        }
	
        public MQService(String requestQueueName)
        {
            MessageReceiverGateway receiver = new MessageReceiverGateway(requestQueueName, GetFormatter());

            RegisterReceiver(receiver);

            this.requestQueue = receiver;

            Console.WriteLine("Processing messages from " + requestQueueName);
        }
			
        protected virtual IMessageFormatter GetFormatter()
        {
            return new XmlMessageFormatter(new Type[] { GetRequestBodyType() });
        }

        protected abstract Type GetRequestBodyType();

        protected Object GetTypedMessageBody(Message msg)
        {
            try 
            {
                if (msg.Body.GetType(). Equals(GetRequestBodyType())) 
                {
                    return msg.Body;
                }
                else
                { 
                    Console.WriteLine("Illegal message format.");
                    return null;
                }
            }
            catch (Exception e) 
            {
                Console.WriteLine("Illegal message format" + e.Message);    
                return null;
            }
        }

        public override void RegisterReceiver(IMessageReceiver<MessageQueue, Message> receiver)
        {
            receiver.ReceiveMessageProcessor += new MessageDelegate<Message>(OnMessageReceived);
        }
	
        public override void Run()
        {
            requestQueue.StartReceivingMessages();
        }

    
        public override void SendReply(Object responseObject, Message originalRequestMessage)
        {
            Message responseMessage = new Message(responseObject);
            responseMessage.CorrelationId = originalRequestMessage.Id;
            responseMessage.AppSpecific = originalRequestMessage.AppSpecific;

            if (originalRequestMessage.ResponseQueue != null) 
            {
                IMessageSender<MessageQueue, Message>  replyQueue = new MessageSenderGateway(originalRequestMessage.ResponseQueue);
                replyQueue.Send(responseMessage);
            }
            else
            {
                invalidQueue.Send(responseMessage);
            }
        }

        public override abstract void OnMessageReceived(Message receivedMessage);
    }
}
