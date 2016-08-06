using MessageGateway;
using Messaging.Base.Interface;
using System;
using System.Collections.Generic;
using System.Messaging;
using System.Text;

namespace MessageGateway
{
    public class RequestReplyService_Synchronous : MQService
    {
        public RequestReplyService_Synchronous(IMessageReceiver<MessageQueue, Message> receiver) : base(receiver) { }
        public RequestReplyService_Synchronous(String requestQueueName) : base(requestQueueName) { }

        protected override Type GetRequestBodyType()
        {
            return typeof(System.String);
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
                    SendReply(outBody, receivedMessage);
                }
            }
        }
    }
}
