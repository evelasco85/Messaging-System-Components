using Messaging.Base;
using System;
using System.Messaging;

namespace MsmqGateway.Core
{
    public class MessageQueueService : QueueService<MessageQueue, Message>
    {
        static protected readonly String InvalidMessageQueueName = ".\\private$\\invalidMessageQueue";

        protected Type requestBodyType;

        public MessageQueueService(IMessageReceiver<MessageQueue, Message> receiver)
            : base(receiver,  new MessageSenderGateway(InvalidMessageQueueName))
        {}
	
        public override void SendReply(Object responseObject, Message originalRequestMessage)
        {
            if (originalRequestMessage.ResponseQueue != null) 
            {
                IMessageSender<MessageQueue, Message>  replyQueue = new MessageSenderGateway(originalRequestMessage.ResponseQueue);

                replyQueue.Send(responseObject,
                    (assignApplicationId, assignCorrelationId) =>
                    {
                        assignApplicationId(originalRequestMessage.AppSpecific.ToString());
                        assignCorrelationId(originalRequestMessage.Id);
                    });
            }
            else
            {
                this.InvalidQueue.Send(responseObject,
                    (assignApplicationId, assignCorrelationId) =>
                    {
                        assignApplicationId(originalRequestMessage.AppSpecific.ToString());
                        assignCorrelationId(originalRequestMessage.Id);
                    });
            }
        }
    }
}
