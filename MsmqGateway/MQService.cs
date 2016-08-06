/* Loan Broker Example with MSMQ
 * from Enterprise Integration Patterns (Addison-Wesley, ISBN 0321200683)
 * 
 * Copyright (c) 2003 Gregor Hohpe 
 *
 * This code is supplied as is. No warranties. 
 */

using Messaging.Base.Interface;
using System;
using System.Messaging;

namespace MessageGateway
{

    public abstract class MQService
    {
        static protected readonly String InvalidMessageQueueName = ".\\private$\\invalidMessageQueue";
        IMessageSender<MessageQueue, Message>invalidQueue = new MessageSenderGateway(InvalidMessageQueueName);

        protected IMessageReceiver<MessageQueue, Message> requestQueue;
        protected Type requestBodyType;
	
        public MQService(IMessageReceiver<MessageQueue, Message> receiver)
        {
            requestQueue = receiver;
            Register(requestQueue);
        }
	
        public MQService(String requestQueueName)
        {
            MessageReceiverGateway q = new MessageReceiverGateway(requestQueueName, GetFormatter());
            Register(q);
            this.requestQueue = q;
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


        public void Register(IMessageReceiver<MessageQueue, Message> rec)
        {
            OnMsgEvent<Message> ev = new OnMsgEvent<Message>(OnMessage);
            rec.OnMessage += ev;
        }
	
        public void Run()
        {
            requestQueue.StartReceivingMessages();
        }

    
        public void SendReply(Object outObj, Message inMsg)
        {
            Message outMsg = new Message(outObj);
            outMsg.CorrelationId = inMsg.Id;
            outMsg.AppSpecific = inMsg.AppSpecific;

            if (inMsg.ResponseQueue != null) 
            {
                IMessageSender<MessageQueue, Message>  replyQueue = new MessageSenderGateway(inMsg.ResponseQueue);
                replyQueue.Send(outMsg);
            }
            else
            {
                invalidQueue.Send(outMsg);
            }
        }

        protected abstract void OnMessage(Message inMsg);

    }

    public class RequestReplyService : MQService
    {
        public RequestReplyService(IMessageReceiver<MessageQueue, Message> receiver) : base(receiver) {}		
        public RequestReplyService(String requestQueueName) : base (requestQueueName) {}

        protected override Type GetRequestBodyType()
        {
            return typeof(System.String);
        }

        protected virtual Object ProcessMessage(Object o)
        {
            String body = (String)o;
            Console.WriteLine("Received Message: " + body);
            return body;
        }

        protected override void OnMessage(Message inMsg)
        {
            inMsg.Formatter =  GetFormatter();
            Object inBody = GetTypedMessageBody(inMsg);
            if (inBody != null) 
            {
                Object outBody = ProcessMessage(inBody);

                if (outBody != null) 
                {
                    SendReply(outBody, inMsg);
                }
            }
        }	
    }

    public class AsyncRequestReplyService : MQService
    {
        public AsyncRequestReplyService(IMessageReceiver<MessageQueue, Message> receiver) : base(receiver) {}		
        public AsyncRequestReplyService(String requestQueueName) : base (requestQueueName) {}


        protected override Type GetRequestBodyType()
        {
            return typeof(System.String);
        }

        protected virtual void ProcessMessage(Object o, Message msg)
        {
            String body = (String)o;
            Console.WriteLine("Received Message: " + body);
        }
    
        protected override void OnMessage(Message inMsg)
        {
            inMsg.Formatter =  GetFormatter();
            Object inBody = GetTypedMessageBody(inMsg);
            if (inBody != null) 
            {
                ProcessMessage(inBody, inMsg);
            }

        }	
    }
}
