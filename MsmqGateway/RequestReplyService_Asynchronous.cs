using MessageGateway;
using Messaging.Base.Interface;
using System;
using System.Collections.Generic;
using System.Messaging;
using System.Text;

namespace MessageGateway
{
    public class RequestReplyService_Asynchronous : MQService
    {
        public RequestReplyService_Asynchronous(IMessageReceiver<MessageQueue, Message> receiver) : base(receiver) { }
        public RequestReplyService_Asynchronous(String requestQueueName) : base(requestQueueName) { }


        protected virtual void ProcessReceivedMessage(Object receivedMessageObject, Message msg)
        {
            String body = (String)receivedMessageObject;
            Console.WriteLine("Received Message: " + body);
        }

        public override void OnMessageReceived(Message receivedMessage)
        {
            receivedMessage.Formatter = GetFormatter();
            Object inBody = GetTypedMessageBody(receivedMessage);
            if (inBody != null)
            {
                ProcessReceivedMessage(inBody, receivedMessage);
            }
        }
    }
}
