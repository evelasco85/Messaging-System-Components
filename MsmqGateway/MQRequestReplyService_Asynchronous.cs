using MessageGateway;
using Messaging.Base;
using Messaging.Base.Constructions;
using System;
using System.Collections.Generic;
using System.Messaging;
using System.Text;

namespace MessageGateway
{
    public abstract class MQRequestReplyService_Asynchronous : RequestReply_Asynchronous<MessageQueue, Message>
    {
        public MQRequestReplyService_Asynchronous(IMessageReceiver<MessageQueue, Message> receiver)
        {
            QueueService = new MQService(receiver);
        }

        public MQRequestReplyService_Asynchronous(String requestQueueName)
        {
            QueueService = new MQService(new MessageReceiverGateway(requestQueueName, GetFormatter()));
        }

        //public virtual void ProcessReceivedMessage(Object receivedMessageObject, Message msg)
        //{
        //    String body = (String)receivedMessageObject;
        //    Console.WriteLine("Received Message: " + body);
        //}

        public override void OnMessageReceived(Message receivedMessage)
        {
            receivedMessage.Formatter = GetFormatter();
            Object inBody = GetTypedMessageBody(receivedMessage);
            if (inBody != null)
            {
                ProcessReceivedMessage(inBody, receivedMessage);
            }
        }

        protected virtual IMessageFormatter GetFormatter()
        {
            return new XmlMessageFormatter(new Type[] { GetRequestBodyType() });
        }

        public virtual Type GetRequestBodyType()
        {
            return typeof(System.String);
        }

        protected Object GetTypedMessageBody(Message msg)
        {
            try
            {
                if (msg.Body.GetType().Equals(GetRequestBodyType()))
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
    }
}
