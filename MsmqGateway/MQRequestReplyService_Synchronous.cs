using MessageGateway;
using Messaging.Base;
using Messaging.Base.Constructions;
using System;
using System.Collections.Generic;
using System.Messaging;
using System.Text;

namespace MessageGateway
{
    public abstract class MQRequestReplyService_Synchronous : RequestReply_Synchronous<MessageQueue, Message>
    {
        public MQRequestReplyService_Synchronous(IMessageReceiver<MessageQueue, Message> receiver)
        {
            QueueService = new MQService(receiver);
        }

        public MQRequestReplyService_Synchronous(String requestQueueName)
        {
            QueueService = new MQService(new MessageReceiverGateway(requestQueueName, GetFormatter()));
        }

        public override void OnMessageReceived(Message receivedMessage)
        {
            receivedMessage.Formatter = GetFormatter();
            Object inBody = GetTypedMessageBody(receivedMessage);

            if (inBody != null)
            {
                Object replyObject = ProcessRequestMessage(inBody);

                if (replyObject != null)
                {
                    QueueService.SendReply(replyObject, receivedMessage);
                }
            }
        }

        public virtual IMessageFormatter GetFormatter()
        {
            return new XmlMessageFormatter(new Type[] { GetRequestBodyType() });
        }

        public virtual Type GetRequestBodyType()
        {
            return typeof(System.String);
        }

        Object GetTypedMessageBody(Message msg)
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
