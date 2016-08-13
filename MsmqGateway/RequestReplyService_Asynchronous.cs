using MessageGateway;
using Messaging.Base.Constructions;
using Messaging.Base.Interface;
using System;
using System.Collections.Generic;
using System.Messaging;
using System.Text;

namespace MessageGateway
{
    public class RequestReplyService_Asynchronous : RequestReply<Message>
    {
        IQueueService<MessageQueue, Message> _queueService;

        public RequestReplyService_Asynchronous(IMessageReceiver<MessageQueue, Message> receiver)// : base(receiver)
        {
            _queueService = new MQService(receiver);

            _queueService.Receiver.ReceiveMessageProcessor += new MessageDelegate<Message>(OnMessageReceived);
        }

        public RequestReplyService_Asynchronous(String requestQueueName)// : base(requestQueueName)
        {
            _queueService = new MQService(new MessageReceiverGateway(requestQueueName, GetFormatter()));

            _queueService.Receiver.ReceiveMessageProcessor += new MessageDelegate<Message>(OnMessageReceived);
        }

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

        public void Run()
        {
            _queueService.Run();
        }

        public void SendReply(Object responseObject, Message originalRequestMessage)
        {
            _queueService.SendReply(responseObject, originalRequestMessage);
        }
    }
}
