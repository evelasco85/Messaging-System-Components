﻿using MessageGateway;
using Messaging.Base.Constructions;
using Messaging.Base.Interface;
using System;
using System.Collections.Generic;
using System.Messaging;
using System.Text;

namespace MessageGateway
{
    public class RequestReplyService_Synchronous : RequestReply<MessageQueue, Message>
    {
        public RequestReplyService_Synchronous(IMessageReceiver<MessageQueue, Message> receiver)
        {
            QueueService = new MQService(receiver);

            RegisterReceiveMessageProcessor();
        }

        public RequestReplyService_Synchronous(String requestQueueName)
        {
            QueueService = new MQService(new MessageReceiverGateway(requestQueueName, GetFormatter()));

            RegisterReceiveMessageProcessor();
        }

        protected virtual Object ProcessReceivedMessage(Object receivedMessageObject)
        {
            String body = (String)receivedMessageObject;
            Console.WriteLine("Received Message: " + body);
            return body;
        }

        public override void OnMessageReceived(Message receivedMessage)
        {
            receivedMessage.Formatter = GetFormatter();
            Object inBody = GetTypedMessageBody(receivedMessage);
            if (inBody != null)
            {
                Object outBody = ProcessReceivedMessage(inBody);

                if (outBody != null)
                {
                    QueueService.SendReply(outBody, receivedMessage);
                }
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
