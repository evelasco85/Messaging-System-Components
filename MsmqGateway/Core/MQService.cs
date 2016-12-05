using Messaging.Base;
using System;
using System.Messaging;

namespace MsmqGateway.Core
{
    public class MQService : QueueService<MessageQueue, Message>
    {
        static protected readonly String InvalidMessageQueueName = ".\\private$\\invalidMessageQueue";

        protected Type requestBodyType;

        public MQService(IMessageReceiver<MessageQueue, Message> receiver)
            : base(receiver,  new MessageSenderGateway(InvalidMessageQueueName))
        {}
	
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
                this.InvalidQueue.Send(responseMessage);
            }
        }
    }
}
