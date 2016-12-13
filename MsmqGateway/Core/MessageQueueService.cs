using Messaging.Base;
using System;
using System.Collections.Generic;
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
                    assignProperty =>
                    {
                        assignProperty("CorrelationId", originalRequestMessage.Id);
                        assignProperty("AppSpecific", originalRequestMessage.AppSpecific);
                    });
            }
            else
            {
                this.InvalidQueue.Send(responseObject,
                     assignProperty =>
                     {
                         assignProperty("CorrelationId", originalRequestMessage.Id);
                         assignProperty("AppSpecific", originalRequestMessage.AppSpecific);
                     });
            }
        }
    }
}
